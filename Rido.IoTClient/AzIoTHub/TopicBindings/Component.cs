using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub.TopicBindings
{
    public class Component<T>
        where T: new()
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
        public async Task<int> UpdateTwinAsync(T instance)
        {
            ComponentValue = instance;
            Dictionary<string, T> dict = new Dictionary<string, T>
            {
                { name, instance }
            };
            return await update.UpdateTwinAsync(dict);
        }
    }
}
