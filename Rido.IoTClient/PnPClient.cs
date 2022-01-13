using MQTTnet.Client;
using System.Threading;

namespace Rido.IoTClient
{
    public class PnPClient
    {
        public IMqttClient Connection;

        public ConnectionSettings ConnectionSettings;

        public PnPClient(IMqttClient c)
        {
            Connection = c;
        }
    }
}
