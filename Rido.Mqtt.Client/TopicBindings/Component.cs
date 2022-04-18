using Rido.MqttCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.Client.TopicBindings
{
    public abstract class Component
    {
        readonly string name;
        readonly UpdatePropertyBinder update;

        public Component(IMqttBaseClient connection, string name)
        {
            this.name = name;
            update = new UpdatePropertyBinder(connection);
        }

        public async Task<int> ReportPropertyAsync(CancellationToken token)
        {

            Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>
                {
                    { name, new Dictionary<string, object>() }
                };
            dict[name] = ToJsonDict();
            dict[name].Add("__t", "c");
            _ = await update.ReportPropertyAsync(dict, token);
            return 0;
        }
        public abstract Dictionary<string, object> ToJsonDict();
    }
}
