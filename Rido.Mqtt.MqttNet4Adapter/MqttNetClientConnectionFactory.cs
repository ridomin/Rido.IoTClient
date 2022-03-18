using MQTTnet;
using MQTTnet.Client;
using Rido.MqttCore;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.MqttNet4Adapter
{
    public class MqttNetClientConnectionFactory : IHubClientConnectionFactory
    {
        public async Task<IMqttBaseClient> CreateHubClientAsync(string connectionSettingsString, CancellationToken cancellationToken = default)
        {
            var connectionSettings = new ConnectionSettings(connectionSettingsString);
            MqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient();
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
    }
}
