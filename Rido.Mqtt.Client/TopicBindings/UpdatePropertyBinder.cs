using Rido.Mqtt.Client;
using Rido.MqttCore;
using Rido.MqttCore.PnP;

using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.Client.TopicBindings
{
    public class UpdatePropertyBinder : IReportPropertyBinder
    {
        readonly IMqttBaseClient connection;
        readonly string name;
        public UpdatePropertyBinder(IMqttBaseClient connection, string propName)
        {
            this.connection = connection;
            this.name = propName;
        }

        public async Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default)
        {
            return await connection.PublishAsync($"pnp/{connection.ClientId}/props/{name}", payload, 1, true, cancellationToken);
        }
    }
}
