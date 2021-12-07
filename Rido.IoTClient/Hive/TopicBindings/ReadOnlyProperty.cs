using MQTTnet.Client;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Hive.TopicBindings
{
    public class ReadOnlyProperty<T>
    {
        readonly UpdatePropertyBinder updateBinder;
        readonly string name;
        readonly string component;

        public T PropertyValue;
        public int Version;

        public ReadOnlyProperty(IMqttClient connection, string name, string component = "")
        {
            updateBinder = new UpdatePropertyBinder(connection);
            this.name = name;
            this.component = component;
        }

        public async Task UpdateTwinPropertyAsync(T newValue, CancellationToken cancellationToken = default)
        {
            PropertyValue = newValue;
            await updateBinder.ReportProperty(ToJson(), cancellationToken);
        }

        string ToJson()
        {
            string result;
            if (string.IsNullOrEmpty(component))
            {
                result = JsonSerializer.Serialize(new Dictionary<string, object> { { name, PropertyValue } });
            }
            else
            {
                Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>
                {
                    { component, new Dictionary<string, object>() }
                };
                dict[component].Add("__t", "c");
                dict[component].Add(name, PropertyValue);
                result = JsonSerializer.Serialize(dict);
            }
            return result;
        }

    }
}
