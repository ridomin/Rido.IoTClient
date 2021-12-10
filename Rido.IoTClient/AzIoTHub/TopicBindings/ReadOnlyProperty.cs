using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub.TopicBindings
{
    public class ReadOnlyProperty<T>
    {
        readonly UpdateTwinBinder updateTwin;
        public string Name { get; private set; }
        readonly string component;
        public T PropertyValue;
        public int Version;

        public ReadOnlyProperty(IMqttClient connection, string name, string component = "")
        {
            updateTwin = new UpdateTwinBinder(connection);
            this.Name = name;
            this.component =  component;
        }

        public async Task UpdateTwinPropertyAsync(T newValue, bool asComponent = false, CancellationToken cancellationToken = default)
        {
            PropertyValue = newValue;
            Version = await updateTwin.UpdateTwinAsync(ToJson(asComponent), cancellationToken);
        }

        string ToJson(bool asComponent = false)
        {
            string result;
            if (asComponent)
            {
                if (string.IsNullOrEmpty(component))
                {
                    throw new ApplicationException("Cant serialize a ReadOnlyProperty as a component if the component name is not set.");
                }

                Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>
                    {
                        { component, new Dictionary<string, object>() }
                    };
                dict[component].Add("__t", "c");
                dict[component].Add(Name, PropertyValue);
                result = JsonSerializer.Serialize(dict);
            }
            else
            {
                result = JsonSerializer.Serialize(new Dictionary<string, object> { { Name, PropertyValue } });
            }
            return result;
        }
    }
}
