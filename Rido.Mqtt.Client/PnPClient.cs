using Rido.MqttCore;
using Rido.PnP.TopicBindings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Rido.Mqtt.Client
{
    public class BirthMessage
    {
        public BirthMessage() => ConnectedTime = DateTime.Now;
        public string ModelId { get; set; }
        public DateTime ConnectedTime { get; private set; }
    }

    public class PnPClient
    {
        public IMqttBaseClient Connection { get; private set; }
        public PnPClient(IMqttBaseClient c)
        {
            Connection = c;
        }

        public async Task<int> Announce(BirthMessage msg, CancellationToken token = default) =>
            await Connection.PublishAsync($"pnp/{Connection.ClientId}/birth", msg, 1, true, token);
    }
}
