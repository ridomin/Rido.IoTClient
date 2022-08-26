using Rido.Mqtt.HubClient.TopicBindings;
using Rido.MqttCore;
using Rido.MqttCore.PnP;

using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient
{
    public class HubMqttClient : IHubMqttClient
    {
        public IMqttConnection Connection { get; set; }

        private readonly IPropertyStoreReader getTwinBinder;
        private readonly IReportPropertyBinder updateTwinBinder;
        private readonly GenericDesiredUpdatePropertyBinder genericDesiredUpdateProperty;
        private readonly GenericCommand command;

       

        public HubMqttClient(IMqttConnection c)
        {
            Connection = c;
            getTwinBinder = new GetTwinBinder(c);
            updateTwinBinder = new UpdateTwinBinder(c);
            command = new GenericCommand(c);
            genericDesiredUpdateProperty = new GenericDesiredUpdatePropertyBinder(c);
        }

        public Func<GenericCommandRequest, Task<CommandResponse>> OnCommandReceived
        {
            get => command.OnCmdDelegate;
            set => command.OnCmdDelegate = value;
        }

        public Func<JsonNode, Task<GenericPropertyAck>> OnPropertyUpdateReceived
        {
            get => genericDesiredUpdateProperty.OnProperty_Updated;
            set => genericDesiredUpdateProperty.OnProperty_Updated = value;
        }

        public Task<string> GetTwinAsync(CancellationToken cancellationToken = default) => getTwinBinder.ReadPropertiesDocAsync(cancellationToken);
        public Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default) => updateTwinBinder.ReportPropertyAsync(payload, cancellationToken);
        public Task<int> SendTelemetryAsync(object payload, CancellationToken t = default) => Connection.PublishAsync($"devices/{Connection.ClientId}/messages/events/", payload, 1, false, t);
        public Task<int> SendTelemetryAsync(object payload, string componentName, CancellationToken t = default) => Connection.PublishAsync($"devices/{Connection.ClientId}/messages/events/?$.sub={componentName}", payload, 1, false, t);

    }
}
