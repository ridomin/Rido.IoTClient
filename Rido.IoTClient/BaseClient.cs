using MQTTnet.Client;
using System.Threading;

namespace Rido.IoTClient
{
    public class BaseClient
    {
        public string InitialState { get; set; } = "";
        public IMqttClient Connection;

        public ConnectionSettings ConnectionSettings { get; set; }

        public BaseClient(IMqttClient c)
        {
            Connection = c;
        }
    }
}
