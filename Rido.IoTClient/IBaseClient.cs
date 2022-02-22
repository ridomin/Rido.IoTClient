using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rido.IoTClient
{
    public interface IBaseClient
    {
        public IMqttClient Connection { get; set; }
        public ConnectionSettings ConnectionSettings { get; }
        public string InitialState { get; set; }
    }
}
