﻿using MQTTnet;
using MQTTnet.Client;
using Rido.Mqtt.MqttNetAdapter;
using Rido.MqttCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient.Dps
{
    public class DpsClient
    {
        readonly MqttClient mqttClient;

        public DpsClient(MqttClient c)
        {
            mqttClient = c;
        }

        public static async Task ProvisionIfNeededAsync(ConnectionSettings dcs)
        {
            if (!string.IsNullOrEmpty(dcs.IdScope))
            {
                DpsStatus dpsResult;
                var mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient();
                DpsClient dpsClient = new DpsClient(mqtt);
                if (!string.IsNullOrEmpty(dcs.SharedAccessKey))
                {
                    dpsResult = await dpsClient.ProvisionWithSasAsync(dcs.IdScope, dcs.DeviceId, dcs.SharedAccessKey, dcs.ModelId);
                }
                else if (!string.IsNullOrEmpty(dcs.X509Key))
                {
                    var segments = dcs.X509Key.Split('|');
                    string pfxpath = segments[0];
                    string pfxpwd = segments[1];
                    dpsResult = await dpsClient.ProvisionWithCertAsync(dcs.IdScope, pfxpath, pfxpwd, dcs.ModelId);
                }
                else
                {
                    throw new ApplicationException("No Key found to provision");
                }

                if (!string.IsNullOrEmpty(dpsResult.RegistrationState.AssignedHub))
                {
                    dcs.HostName = dpsResult.RegistrationState.AssignedHub;
                }
                else
                {
                    throw new ApplicationException("DPS Provision failed: " + dpsResult.Status);
                }
            }
        }


        public async Task<DpsStatus> ProvisionWithCertAsync(string idScope, string pfxPath, string pfxPwd, string modelId = "")
        {
            //if (mqttClient.IsConnected)
            //{
            //    await mqttClient.DisconnectAsync();
            //}

            var tcs = new TaskCompletionSource<DpsStatus>();

            X509Certificate2 cert = new X509Certificate2(pfxPath, pfxPwd);
            var registrationId = cert.SubjectName.Name[3..];
            var resource = $"{idScope}/registrations/{registrationId}";
            var username = $"{resource}/api-version=2019-03-31";

            var options = new MqttClientOptionsBuilder()
                .WithClientId(registrationId)
                .WithTcpServer("global.azure-devices-provisioning.net", 8883)
                .WithCredentials(username)
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = true,
                    CertificateValidationHandler = (x) => { return true; },
                    SslProtocol = SslProtocols.Tls12,
                    Certificates = new List<X509Certificate> { cert }
                })
                .Build();

            await mqttClient.ConnectAsync(options);

            var suback = await mqttClient.SubscribeAsync("$dps/registrations/res/#");
            suback.Items.ToList().ForEach(x => Trace.TraceWarning($"+ {x.TopicFilter.Topic} {x.ResultCode}"));
            await ConfigureDPSFlowAsync(registrationId, modelId, tcs);

            return tcs.Task.Result;
        }

        public async Task<DpsStatus> ProvisionWithSasAsync(string idScope, string registrationId, string sasKey, string modelId = "")
        {
            if (mqttClient.IsConnected)
            {
                await mqttClient.DisconnectAsync();
            }
            var tcs = new TaskCompletionSource<DpsStatus>(TaskCreationOptions.RunContinuationsAsynchronously);

            var resource = $"{idScope}/registrations/{registrationId}";
            var username = $"{resource}/api-version=2019-03-31";
            var password = SasAuth.CreateSasToken(resource, sasKey, 5);

            var options = new MqttClientOptionsBuilder()
                .WithClientId(registrationId)
                .WithTcpServer("global.azure-devices-provisioning.net", 8883)
                .WithCredentials(username, password)
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = true,
                    CertificateValidationHandler = (x) => { return true; },
                    SslProtocol = SslProtocols.Tls12
                })
            .Build();

            await mqttClient.ConnectAsync(options);

            var suback = await mqttClient.SubscribeAsync("$dps/registrations/res/#");
            suback.Items.ToList().ForEach(x => Trace.TraceWarning($"+ {x.TopicFilter.Topic} {x.ResultCode}"));
            await ConfigureDPSFlowAsync(registrationId, modelId, tcs);
            return await tcs.Task.TimeoutAfter(TimeSpan.FromSeconds(60));
        }

        private async Task ConfigureDPSFlowAsync(string registrationId, string modelId, TaskCompletionSource<DpsStatus> tcs)
        {

            int rid = RidCounter.NextValue();
            string msg = string.Empty;
            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var topic = e.ApplicationMessage.Topic;

                if (e.ApplicationMessage.Payload != null)
                {
                    msg = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                }

                if (e.ApplicationMessage.Topic.StartsWith($"$dps/registrations/res/"))
                {
                    var topicSegments = topic.Split('/');
                    int reqStatus = Convert.ToInt32(topicSegments[3]);
                    if (reqStatus >= 400)
                    {
                        tcs.SetException(new ApplicationException(msg));
                    }
                    var dpsRes = JsonSerializer.Deserialize<DpsStatus>(msg);

                    if (dpsRes.Status == "failed")
                    {
                        tcs.SetException(new ApplicationException(dpsRes.RegistrationState.ErrorMessage));
                    }    

                    if (dpsRes.Status == "assigning")
                    {
                        // TODO: ready retry-after
                        await Task.Delay(2500); //avoid throtling
                        var pollTopic = $"$dps/registrations/GET/iotdps-get-operationstatus/?$rid={rid}&operationId={dpsRes.OperationId}";
                        var puback = await mqttClient.PublishAsync(new MqttApplicationMessage() { Topic = pollTopic });
                    }
                    else
                    {
                        tcs.TrySetResult(dpsRes);

                    }
                }
            };
            var putTopic = $"$dps/registrations/PUT/iotdps-register/?$rid={rid}";
            var puback = await mqttClient.PublishAsync(
                new MqttApplicationMessageBuilder()
                    .WithTopic(putTopic)
                    .WithPayload(JsonSerializer.Serialize(new { registrationId, payload = new { modelId } })).Build());

        }
    }
}