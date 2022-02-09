using MQTTnet.Client;
using System.Threading;

namespace Rido.IoTClient
{
    public class BaseClient
    {
        public string InitialState = "";
        public IMqttClient Connection;

        public ConnectionSettings ConnectionSettings;

        public BaseClient(IMqttClient c)
        {
            Connection = c;
        }
    }
}
