using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Implementations;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Hive
{
    public class HiveClient
    {
        public IMqttClient Connection;
        public string InitialTwin = string.Empty;

        public ConnectionSettings ConnectionSettings;

        public HiveClient(IMqttClient c)
        {
            this.Connection = c;
        }

        protected static async Task<IMqttClient> CreateAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            IMqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient(new MqttClientAdapterFactory());
            var connAck = await mqtt.ConnectAsync(new MqttClientOptionsBuilder()
                .WithTcpServer(cs.HostName, 8883)
                .WithTls()
                .WithClientId(cs.DeviceId)
                .WithCredentials(cs.DeviceId, cs.SharedAccessKey)
                .Build(), cancellationToken);
            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }
            return mqtt;
        }
    }
}
