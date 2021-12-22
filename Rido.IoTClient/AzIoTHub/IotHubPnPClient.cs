using MQTTnet.Client;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub
{
    public class IoTHubPnPClient : PnPClient 
    {
        public string InitialState = string.Empty;
        readonly IPropertyStoreReader getTwinBinder;
        readonly IReportPropertyBinder updateTwinBinder;

        public IoTHubPnPClient(IMqttClient connection) : base(connection)
        {
            getTwinBinder = GetTwinBinder.GetInstance(connection);
            updateTwinBinder = UpdateTwinBinder.GetInstance(connection);
        }

        public Task<string> GetTwinAsync(CancellationToken cancellationToken = default) => getTwinBinder.ReadPropertiesDocAsync(cancellationToken);

        public Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default) => updateTwinBinder.ReportPropertyAsync(payload, cancellationToken);

    }
}
