using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub.TopicBindings
{
    public class Component<T>
        where T: ITwinSerializable, new()
    {
        readonly string name;
        readonly UpdateTwinBinder update;
        public T ComponentValue { get; set; }

        public Component(IMqttClient connection, string name)
        {
            this.name = name;
            this.ComponentValue = new T();
            update = new UpdateTwinBinder(connection);
        }
        public Task<int> UpdateTwinAsync() => UpdateTwinAsync(ComponentValue);

        public async Task<int> UpdateTwinAsync(T instance)
        {
            ComponentValue = instance;
            Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>
                {
                    { name, new Dictionary<string, object>() }
                };
            dict[name] = ComponentValue.ToJsonDict();
            dict[name].Add("__t", "c");
            return await update.UpdateTwinAsync(dict);
        }
    }
}
