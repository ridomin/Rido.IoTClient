using Rido.MqttCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient.TopicBindings
{
    public class ReadOnlyProperty<T> : IReadOnlyProperty<T>
    {
        readonly IPropertyStoreWriter updateTwin;
        public readonly string Name;
        readonly string component;
        public T PropertyValue { get; set; }
        public int Version { get; set; }

        public ReadOnlyProperty(IMqttBaseClient connection, string name, string component = "")
        {
            updateTwin = new UpdateTwinBinder(connection);
            Name = name;
            this.component = component;
        }

        public async Task<int> ReportPropertyAsync(CancellationToken cancellationToken = default)
        {
            bool asComponent = !string.IsNullOrEmpty(component);
            Version = await updateTwin.ReportPropertyAsync(JsonSerializer.Serialize(ToJsonDict(asComponent)), cancellationToken);
            return Version;
        }

        Dictionary<string, object> ToJsonDict(bool asComponent = false)
        {
            Dictionary<string, object> result;
            if (asComponent == false)
            {
                result = new Dictionary<string, object> { { Name, PropertyValue } };
            }
            else
            {
                Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>
                {
                    { component, new Dictionary<string, object>() }
                };
                dict[component].Add("__t", "c");
                dict[component].Add(Name, PropertyValue);
                result = dict.ToDictionary(pair => pair.Key, pair => (object)pair.Value);
            }
            return result;
        }

    }
}
