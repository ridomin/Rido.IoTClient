using Rido.MqttCore;
using static Rido.MqttCore.Birth.BirthConvention;

namespace Rido.Mqtt.Client
{
    public class PnPClient
    {
        public IMqttBaseClient Connection { get; private set; }
        public PnPClient(IMqttBaseClient c)
        {
            Connection = c;
            _ = Connection.PublishAsync(
                BirthTopic(Connection.ClientId), 
                new BirthMessage(ConnectionStatus.online)
                {
                    ModelId = c.ConnectionSettings.ModelId
                }, 
                1, true);
        }
    }
}
