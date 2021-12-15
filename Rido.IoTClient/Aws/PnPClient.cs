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
        readonly UpdateShadowBinder updateShadowBinder;
        //public readonly DesiredUpdatePropertyBinder<string> desiredUpdatePropertyBinder;

        public PnPClient(IMqttClient c, ConnectionSettings cs)
        {
            this.Connection = c;
            this.ConnectionSettings = cs;
            getShadowBinder = new GetShadowBinder(c, ConnectionSettings.DeviceId);
            updateShadowBinder = new UpdateShadowBinder(c, cs.DeviceId);
            //desiredUpdatePropertyBinder = new DesiredUpdatePropertyBinder<string>(c, cs.DeviceId, "name");
        }

        public static async Task<PnPClient> CreateAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
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

            return new PnPClient(mqtt, cs);
        }

        public Task<string> GetShadowAsync(CancellationToken cancellationToken = default) => getShadowBinder.GetShadow(cancellationToken);
        public Task<int> UpdateShadowAsync(object payload, CancellationToken cancellationToken = default) => updateShadowBinder.UpdateShadowAsync(payload, cancellationToken);
    }
}
