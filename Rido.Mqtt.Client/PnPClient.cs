using Rido.MqttCore;
using System.Threading;
using System.Threading.Tasks;
using static Rido.MqttCore.Birth.BirthConvention;

namespace Rido.Mqtt.Client
{
    public class PnPClient
    {
        public IMqttBaseClient Connection { get; private set; }
        public PnPClient(IMqttBaseClient c)
        {
            Connection = c;
        }

        public async Task<int> Announce(BirthMessage msg, CancellationToken token = default) =>
            await Connection.PublishAsync(BirthTopic(Connection.ClientId), msg, 1, true, token);
    }
}
