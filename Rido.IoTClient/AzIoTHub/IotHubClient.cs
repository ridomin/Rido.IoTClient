using MQTTnet.Client;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub
{
    public class IoTHubClient : BaseClient 
    {
      
        readonly IPropertyStoreReader getTwinBinder;
        readonly IReportPropertyBinder updateTwinBinder;

        public IoTHubClient(IMqttClient connection) : base(connection)
        {
            getTwinBinder = new GetTwinBinder(connection);
            updateTwinBinder = new UpdateTwinBinder(connection);
        }

        public Task<string> GetTwinAsync(CancellationToken cancellationToken = default) => getTwinBinder.ReadPropertiesDocAsync(cancellationToken);

        public Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default) => updateTwinBinder.ReportPropertyAsync(payload, cancellationToken);

    }
}
