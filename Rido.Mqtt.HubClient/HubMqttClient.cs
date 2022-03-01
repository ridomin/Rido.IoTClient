using Rido.Mqtt.HubClient.TopicBindings;
using Rido.MqttCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient
{
    public  class HubMqttClient
    {
        public IMqttBaseClient Connection { get; set; }
        public ConnectionSettings ConnectionSettings { get; set; }

        readonly IPropertyStoreReader getTwinBinder;

        public HubMqttClient(IMqttBaseClient c)
        {
            Connection = c;
            getTwinBinder = new GetTwinBinder(c);
        }

        public Task<string> GetTwinAsync(CancellationToken cancellationToken = default) => getTwinBinder.ReadPropertiesDocAsync(cancellationToken);

        public Task<int> SendTelemetryAsync(string payload, CancellationToken t = default) =>
            Connection.PublishAsync($"devices/{Connection.ClientId}/messages/events/", payload, 0, t);
    }
}
