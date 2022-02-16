using MQTTnet.Client;
using Rido.IoTClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.PnPMqtt.TopicBindings
{
    public class ReadOnlyProperty<T> : IReadOnlyProperty<T>
    {
        readonly IReportPropertyBinder updateBinder;
        public string Name;
        readonly string component;

        public T PropertyValue { get; set; }
        public int Version;

        public ReadOnlyProperty(IMqttClient connection, string name, string component = "")
        {
            updateBinder = new UpdatePropertyBinder(connection);
            Name = name;
            this.component = component;
        }

        public async Task<int> ReportPropertyAsync(CancellationToken cancellationToken = default)
        {
            bool asComponent = !string.IsNullOrEmpty(component);
            await updateBinder.ReportPropertyAsync(ToJsonDict(asComponent), cancellationToken);
            return -1;
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
