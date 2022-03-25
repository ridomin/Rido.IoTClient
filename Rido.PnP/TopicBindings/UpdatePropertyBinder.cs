using Rido.MqttCore;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.PnP.TopicBindings
{
    public class UpdatePropertyBinder : IReportPropertyBinder
    {
        readonly IMqttBaseClient connection;

        public UpdatePropertyBinder(IMqttBaseClient connection)
        {
            this.connection = connection;
        }

        public async Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default)
        {
            return await connection.PublishAsync($"pnp/{connection.ClientId}/props/reported", payload, 1, true, cancellationToken);
        }
    }
}
