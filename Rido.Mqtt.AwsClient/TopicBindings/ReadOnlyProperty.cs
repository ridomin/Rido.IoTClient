using Rido.MqttCore;
using Rido.PnP;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.AwsClient.TopicBindings
{
    public class ReadOnlyProperty<T> : IReadOnlyProperty<T>
    {
        private readonly IReportPropertyBinder updateBinder;
        public string Name;
        private readonly string component;


        public T PropertyValue { get; set; }
        public int Version { get; set; }

        public ReadOnlyProperty(IMqttBaseClient connection, string name, string component = "")
        {
            updateBinder = new UpdateShadowBinder(connection);
            Name = name;
            this.component = component;
        }

        public async Task<int> ReportPropertyAsync(CancellationToken cancellationToken = default)
        {
            bool asComponent = !string.IsNullOrEmpty(component);
            return await updateBinder.ReportPropertyAsync(ToJsonDict(asComponent), cancellationToken);
        }

        private Dictionary<string, object> ToJsonDict(bool asComponent = false)
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
