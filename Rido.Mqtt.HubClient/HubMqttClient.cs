using Rido.Mqtt.HubClient.TopicBindings;
using Rido.MqttCore;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient
{
    public class HubMqttClient
    {
        public IMqttBaseClient Connection { get; set; }
        public ConnectionSettings ConnectionSettings => Connection.ConnectionSettings;

        private readonly IPropertyStoreReader getTwinBinder;
        private readonly IReportPropertyBinder updateTwinBinder;
        public GenericDesiredUpdatePropertyBinder genericDesiredUpdateProperty;
        public GenericCommand Command;
        public HubMqttClient(IMqttBaseClient c)
        {
            Connection = c;
            getTwinBinder = new GetTwinBinder(c);
            updateTwinBinder = new UpdateTwinBinder(c);
            Command = new GenericCommand(c);
            genericDesiredUpdateProperty = new GenericDesiredUpdatePropertyBinder(c);
        }

        public Task<string> GetTwinAsync(CancellationToken cancellationToken = default) => getTwinBinder.ReadPropertiesDocAsync(cancellationToken);
        public Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default) => updateTwinBinder.ReportPropertyAsync(payload, cancellationToken);

        public Task<int> SendTelemetryAsync(object payload, CancellationToken t = default) =>
            Connection.PublishAsync($"devices/{Connection.ClientId}/messages/events/", payload, 0, t);
    }
}
