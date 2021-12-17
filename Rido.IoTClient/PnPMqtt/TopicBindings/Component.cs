using MQTTnet.Client;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.PnPMqtt.TopicBindings
{
    public class Component<T>
        where T : ITwinSerializable, new()
    {
        readonly string name;
        readonly UpdatePropertyBinder update;
        public T ComponentValue { get; set; }

        public Component(IMqttClient connection, string name)
        {
            this.name = name;
            ComponentValue = new T();
            update = UpdatePropertyBinder.GetInstance(connection);
        }
        public Task<int> UpdateTwinAsync(CancellationToken token = default) => UpdateTwinAsync(ComponentValue, token);

        public async Task<int> UpdateTwinAsync(T instance, CancellationToken token)
        {
            ComponentValue = instance;
            Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>
                {
                    { name, new Dictionary<string, object>() }
                };
            dict[name] = ComponentValue.ToJsonDict();
            dict[name].Add("__t", "c");
            _ = await update.ReportPropertyAsync(dict, token);
            return 0;
        }
    }
}
