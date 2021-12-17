using MQTTnet;
using MQTTnet.Client;
using Rido.IoTClient.Aws.TopicBindings;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Aws
{
    public class AwsPnPClient
    {
        public IMqttClient Connection;
        public string InitialTwin = string.Empty;

        public ConnectionSettings ConnectionSettings;
        readonly GetShadowBinder getShadowBinder;
        readonly IReportPropertyBinder updateShadowBinder;
        public readonly DesiredUpdatePropertyBinder<string> desiredUpdatePropertyBinder;

        public AwsPnPClient(IMqttClient c, ConnectionSettings cs)
        {
            this.Connection = c;
            this.ConnectionSettings = cs;
            getShadowBinder = new GetShadowBinder(c, ConnectionSettings.ClientId);
            updateShadowBinder = UpdateShadowBinder.GetInstance(c);
            desiredUpdatePropertyBinder = new DesiredUpdatePropertyBinder<string>(c, cs.DeviceId, "name");
        }

        public static async Task<AwsPnPClient> CreateAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            IMqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient();
            var connAck = await mqtt.ConnectAsync(
                new MqttClientOptionsBuilder().WithAwsX509Credentials(cs).Build(),
                cancellationToken);

            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }

            return new AwsPnPClient(mqtt, cs);
        }

        public Task<string> GetShadowAsync(CancellationToken cancellationToken = default) => getShadowBinder.GetShadow(cancellationToken);
        public Task<int> UpdateShadowAsync(object payload, CancellationToken cancellationToken = default) => updateShadowBinder.ReportPropertyAsync(payload, cancellationToken);
    }
}
