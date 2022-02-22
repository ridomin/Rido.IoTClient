using MQTTnet.Client;
using System.Dynamic;
using System.Threading;

namespace Rido.IoTClient
{
    public class BaseClient : IBaseClient
    {
        public string InitialState { get; set; } = "";
        public IMqttClient Connection { get;set; }
        public ConnectionSettings ConnectionSettings { get; set; }

        public BaseClient(IMqttClient c)
        {
            Connection = c;
        }
    }
}
