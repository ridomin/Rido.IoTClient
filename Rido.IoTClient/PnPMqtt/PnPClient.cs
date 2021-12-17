using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Implementations;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.PnPMqtt
{
    public class PnPClient
    {
        public IMqttClient Connection;
        public string InitialState = string.Empty;

        public ConnectionSettings ConnectionSettings;

        public PnPClient(IMqttClient c)
        {
            Connection = c;
        }

        public static async Task<PnPClient> CreateAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            IMqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient();
            var connAck = await mqtt.ConnectAsync(new MqttClientOptionsBuilder().WithBasicAuth(cs).Build(), cancellationToken);
            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }
            return new PnPClient(mqtt)
            {
                ConnectionSettings = cs
            };
        }

    }
}
