using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Implementations;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Hive
{
    public class PnPClient
    {
        public IMqttClient Connection;
        public string InitialTwin = string.Empty;

        public ConnectionSettings ConnectionSettings;

        public PnPClient(IMqttClient c)
        {
            this.Connection = c;
        }

        public static async Task<PnPClient> CreateAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
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
            return new PnPClient(mqtt)
            {
                ConnectionSettings = cs
            };
        }

        public Task<string> GetTwinAsync(CancellationToken cancellationToken = default) => Task.FromResult("");
    }
}
