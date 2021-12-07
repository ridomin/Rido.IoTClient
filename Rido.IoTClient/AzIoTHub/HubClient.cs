using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Implementations;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub
{
    public class HubClient
    {
        public IMqttClient connection;
        public ConnectionSettings connectionSettings;
        public string InitialTwin = string.Empty;

        protected GetTwinBinder GetTwinBinder;
        protected UpdateTwinBinder UpdateTwinBinder;

        public HubClient(IMqttClient connection, ConnectionSettings cs)
        {
            this.connection = connection;
            this.connectionSettings = cs;
            GetTwinBinder = new GetTwinBinder(connection);
            UpdateTwinBinder = new UpdateTwinBinder(connection);
        }

        public async Task<string> GetTwinAsync(CancellationToken cancellationToken = default) => await GetTwinBinder.GetTwinAsync(cancellationToken);

        public async Task<int> UpdateTwinAsync(object payload, CancellationToken cancellationToken = default) => await UpdateTwinBinder.UpdateTwinAsync(payload, cancellationToken);

        protected static async Task<IMqttClient> ConnectWithConnectsionSettings(ConnectionSettings cs, CancellationToken cancellationToken)
        {
            IMqttClient mqtt = new MqttFactory().CreateMqttClient(new MqttClientAdapterFactory());
            var connAck = await mqtt.ConnectAsync(new MqttClientOptionsBuilder().WithAzureIoTHubCredentials(cs).Build(), cancellationToken);
            return mqtt;
        }
    }
}
