using MQTTnet.Client;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub
{
    public class IoTHubPnPClient //: IDisposable
    {
        public readonly IMqttClient Connection;

        public ConnectionSettings ConnectionSettings;
        public string InitialTwin = string.Empty;

        readonly IPropertyStoreReader getTwinBinder;
        readonly IReportPropertyBinder updateTwinBinder;

        public IoTHubPnPClient(IMqttClient connection)
        {
            this.Connection = connection;
            getTwinBinder = GetTwinBinder.GetInstance(connection);
            updateTwinBinder = UpdateTwinBinder.GetInstance(connection);
        }

        public Task<string> GetTwinAsync(CancellationToken cancellationToken = default) => getTwinBinder.ReadPropertiesDocAsync(cancellationToken);

        public Task<int> UpdateTwinAsync(object payload, CancellationToken cancellationToken = default) => updateTwinBinder.ReportPropertyAsync(payload, cancellationToken);

    }
}
