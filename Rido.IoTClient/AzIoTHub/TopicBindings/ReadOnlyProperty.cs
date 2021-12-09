using MQTTnet.Client;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub.TopicBindings
{
    public class ReadOnlyProperty<T>
    {
        readonly UpdateTwinBinder updateTwin;
        readonly string name;

        public T PropertyValue;
        public int Version;

        public ReadOnlyProperty(IMqttClient connection, string name)
        {
            updateTwin = new UpdateTwinBinder(connection);
            this.name = name;
        }

        public async Task UpdateTwinPropertyAsync(T newValue, CancellationToken cancellationToken = default)
        {
            PropertyValue = newValue;
            Version = await updateTwin.UpdateTwinAsync(ToJson(), cancellationToken);
        }

        string ToJson()
        {
            return JsonSerializer.Serialize(new Dictionary<string, object> { { name, PropertyValue } });
            //{
            //    Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>
            //    {
            //        { component, new Dictionary<string, object>() }
            //    };
            //    dict[component].Add("__t", "c");
            //    dict[component].Add(name, PropertyValue);
            //    result = JsonSerializer.Serialize(dict);
            //}
        }

    }
}
