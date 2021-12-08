using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rido.IoTCore.IoTHub.Tests
{
    internal class SampleDevice : HubClient
    {
        private SampleDevice(IMqttClient c) : base(c)
        {

        }

        public  static async Task<SampleDevice> CreateAsync(ConnectionSettings cs)
        {
            var mqtt = await HubClient.CreateAsync(cs);
            return new SampleDevice(mqtt);
        }
    }
}
