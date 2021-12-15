using MQTTnet.Client;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Hive.TopicBindings
{
    public class ReadOnlyProperty<T>
    {
        readonly UpdatePropertyBinder updateBinder;
        public string Name;
        readonly string component;

        public T PropertyValue;
        public int Version;

        public ReadOnlyProperty(IMqttClient connection, string name, string component = "")
        {
            updateBinder = new UpdatePropertyBinder(connection);
            this.Name = name;
            this.component = component;
        }

        public async Task UpdateTwinPropertyAsync(T newValue, bool asComponent = false, CancellationToken cancellationToken = default)
        {
            PropertyValue = newValue;
            await updateBinder.UpdatePropertyAsync(ToJsonDict(asComponent), cancellationToken);
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
