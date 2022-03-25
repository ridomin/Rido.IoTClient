using Rido.MqttCore;
using Rido.PnP.TopicBindings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Rido.PnP
{
    public class BirthMessage
    {
        public string ModelId { get; set; }
    }

    public class PnPClient
    {
        IMqttBaseClient connection;
        public PnPClient(IMqttBaseClient c)
        {
            connection = c;
        }

        public async Task<int> Announce(BirthMessage msg, CancellationToken token = default) =>
            await connection.PublishAsync($"pnp/{connection.ClientId}/birth", msg, 1,true, token);
    }
}
