using MQTTnet;
using MQTTnet.Client;
using Rido.IoTClient.Aws.TopicBindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Aws
{
    public class PnPClient
    {
        public IMqttClient Connection;
        public string InitialTwin = string.Empty;

        public ConnectionSettings ConnectionSettings;
        readonly GetShadowBinder getShadowBinder;

        public PnPClient(IMqttClient c, ConnectionSettings cs)
        {
            this.Connection = c;
            this.ConnectionSettings = cs;
            getShadowBinder = new GetShadowBinder(c, ConnectionSettings.DeviceId);
        }

        public static async Task<PnPClient> CreateAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            var segments = cs.X509Key.Split('|');
            var cert = new X509Certificate2(segments[0], segments[1], X509KeyStorageFlags.Exportable);

            IMqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient();
            var connAck = await mqtt.ConnectAsync(
                new MqttClientOptionsBuilder()
                    .WithTcpServer(cs.HostName, 8883)
                    .WithKeepAlivePeriod(new TimeSpan(0, 0, 0, 300))
                    .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
                    .WithClientId(cs.ClientId)
                    .WithCleanSession(false)
                    .WithTls(new MqttClientOptionsBuilderTlsParameters
                    {
                        UseTls = true,
                        AllowUntrustedCertificates = false,
                        IgnoreCertificateChainErrors = false,
                        IgnoreCertificateRevocationErrors = false,
                        SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
                        Certificates = new List<X509Certificate> { cert },
                    })
                    .Build(),
                cancellationToken);

            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }

            return new PnPClient(mqtt, cs);
        }

        //TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
        //public async Task<string> GetShadow()
        //{
        //    string topic = "$aws/things/TheThing/shadow/get";
        //    var suback = await Connection.SubscribeAsync(
        //        new MqttTopicFilterBuilder()
        //            .WithTopic(topic + "/accepted")
        //            .Build(),
        //        new MqttTopicFilterBuilder()
        //            .WithTopic(topic + "/rejected")
        //            .Build()
        //        );
        //    suback.Items.ToList().ForEach(x => Trace.TraceInformation($"SUB {x.TopicFilter} {x.ResultCode}"));
        //    Connection.ApplicationMessageReceivedAsync += async m =>
        //    {
        //        if (m.ApplicationMessage.Topic.Contains("/get/accepted"))
        //        {
        //            var msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload);
        //            tcs.SetResult(msg);
        //        }
        //        await Task.Yield();
        //    };
        //    var puback = await Connection.PublishAsync(topic);
        //    await Task.Delay(100);
        //    Console.WriteLine("PUB " + puback.ReasonCode);
        //    return await tcs.Task.TimeoutAfter(TimeSpan.FromSeconds(5));
        //}


        public Task<string> GetShadowAsync(CancellationToken cancellationToken = default) => getShadowBinder.GetShadow(cancellationToken);
    }
}
