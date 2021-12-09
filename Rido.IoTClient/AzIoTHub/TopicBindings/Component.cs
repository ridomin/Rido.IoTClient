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
        public T CV { get; set; }

        public Component(IMqttClient connection, string name)
        {
            this.name = name;
            this.CV = new T();
            update = new UpdateTwinBinder(connection);
        }
        public Task<int> UpdateTwinAsync() => UpdateTwinAsync(CV);

        public async Task<int> UpdateTwinAsync(T instance)
        {
            CV = instance;
            Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>
                {
                    { name, new Dictionary<string, object>() }
                };
            dict[name] = CV.ToJson();
            dict[name].Add("__t", "c");
            return await update.UpdateTwinAsync(dict);
        }
    }
}
