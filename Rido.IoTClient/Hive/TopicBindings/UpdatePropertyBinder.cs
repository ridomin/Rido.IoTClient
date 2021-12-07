using MQTTnet.Client;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Hive.TopicBindings
{
    public class UpdatePropertyBinder
    {
        readonly IMqttClient connection;

        public UpdatePropertyBinder(IMqttClient connection)
        {
            this.connection = connection;
        }

        public async Task<MqttClientPublishResult> ReportProperty(object payload, CancellationToken cancellationToken = default) =>
            await connection.PublishAsync($"pnp/{connection.Options.ClientId}/props/reported", payload, cancellationToken);
    }
}
