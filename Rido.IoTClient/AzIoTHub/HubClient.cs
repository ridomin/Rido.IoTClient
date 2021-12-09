using MQTTnet;
using MQTTnet.Client;
using Rido.IoTClient.AzDps;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub
{
    public class HubClient
    {
        public readonly IMqttClient Connection;

        public ConnectionSettings ConnectionSettings;
        public string InitialTwin = string.Empty;

        readonly GetTwinBinder GetTwinBinder;
        readonly UpdateTwinBinder UpdateTwinBinder;

        public HubClient(IMqttClient connection)
        {
            this.Connection = connection;
            GetTwinBinder = new GetTwinBinder(connection);
            UpdateTwinBinder = new UpdateTwinBinder(connection);
        }

        public async Task<string> GetTwinAsync(CancellationToken cancellationToken = default) => 
            await GetTwinBinder.GetTwinAsync(cancellationToken);

        public async Task<int> UpdateTwinAsync(object payload, CancellationToken cancellationToken = default) => 
            await UpdateTwinBinder.UpdateTwinAsync(payload, cancellationToken);

        public static async Task<HubClient> CreateAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            await DpsClient.ProvisionIfNeededAsync(cs);
            IMqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient();
            var connAck = await mqtt.ConnectAsync(new MqttClientOptionsBuilder().WithAzureIoTHubCredentials(cs).Build(), cancellationToken);
            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }
            return new HubClient(mqtt) { ConnectionSettings = cs} ;
        }
    }
}
