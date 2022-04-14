using Rido.MqttCore;
using Rido.MqttCore.PnP;

using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient.TopicBindings
{
    public abstract class Component
    {
        private readonly string name;
        private readonly IPropertyStoreWriter update;

        public Component(IMqttBaseClient connection, string name)
        {
            this.name = name;
            update = new UpdateTwinBinder(connection);
        }

        public async Task<int> ReportPropertyAsync(CancellationToken token = default)
        {
            Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>
                {
                    { name, new Dictionary<string, object>() }
                };
            dict[name] = ToJsonDict();
            dict[name].Add("__t", "c");
            var v = await update.ReportPropertyAsync(dict, token);
            return v;
        }

        public abstract Dictionary<string, object> ToJsonDict();
    }
}
