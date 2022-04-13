using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using Rido.MqttCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.MqttNet3Adapter
{
    public class MqttNetClientConnectionFactory : IHubClientConnectionFactory
    {
        public async Task<IMqttBaseClient> CreateHubClientAsync(string connectionSettingsString, CancellationToken cancellationToken = default)
        {
            var connectionSettings = new ConnectionSettings(connectionSettingsString);
            return await CreateHubClientAsync(connectionSettings, cancellationToken);
        }

        public async Task<IMqttBaseClient> CreateHubClientAsync(ConnectionSettings connectionSettings, CancellationToken cancellationToken = default)
        {
            MqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient() as MqttClient;
            var connAck = await mqtt.ConnectAsync(
                new MqttClientOptionsBuilder()
                    .WithAzureIoTHubCredentials(connectionSettings)
                    .Build(),
                cancellationToken);

            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }

            return new MqttNetClient(mqtt) { ConnectionSettings = connectionSettings };
        }

        public static async Task<IMqttBaseClient> CreateAwsClientAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            MqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient() as MqttClient;
            var connAck = await mqtt.ConnectAsync(new MqttClientOptionsBuilder().WithAwsX509Credentials(cs).Build(), cancellationToken);
            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }
            return new MqttNetClient(mqtt) { ConnectionSettings = cs };
        }

        public async Task<IMqttBaseClient> CreateBasicClientAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            MqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient() as MqttClient;
            var connack = await mqtt.ConnectAsync(new MqttClientOptionsBuilder()
                .WithBasicAuth(cs)
                .Build(), cancellationToken);
            if (connack.ResultCode != MqttClientConnectResultCode.Success)
            {
                throw new ApplicationException(connack.ReasonString);
            }
            return new MqttNetClient(mqtt) { ConnectionSettings = cs };
        }
        public async Task<IMqttBaseClient> CreateDpsClientAsync(string connectionSettingsString, CancellationToken cancellationToken = default)
        {
            MqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient() as MqttClient;
            var cs = new ConnectionSettings(connectionSettingsString);
            if (cs.Auth == "SAS")
            {
                var resource = $"{cs.IdScope}/registrations/{cs.DeviceId}";
                var username = $"{resource}/api-version=2019-03-31";
                var password = SasAuth.CreateSasToken(resource, cs.SharedAccessKey, 5);
                var options = new MqttClientOptionsBuilder()
                    .WithClientId(cs.DeviceId)
                    .WithTcpServer("global.azure-devices-provisioning.net", 8883)
                    .WithCredentials(username, password)
                    .WithTls(new MqttClientOptionsBuilderTlsParameters
                    {
                        UseTls = true,
                        CertificateValidationHandler = (x) => { return true; },
                        SslProtocol = SslProtocols.Tls12
                    })
                .Build();
                await mqtt.ConnectAsync(options, cancellationToken);
            } 
            else if (cs.Auth == "X509")
            {
                var segments = cs.X509Key.Split('|');
                string pfxpath = segments[0];
                string pfxpwd = segments[1];
                X509Certificate2 cert = new X509Certificate2(pfxpath, pfxpwd);
                var registrationId = cert.SubjectName.Name[3..];
                var resource = $"{cs.IdScope}/registrations/{registrationId}";
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

                await mqtt.ConnectAsync(options, cancellationToken);
            }
            return new MqttNetClient(mqtt) { ConnectionSettings = cs};
        }
    }
}
