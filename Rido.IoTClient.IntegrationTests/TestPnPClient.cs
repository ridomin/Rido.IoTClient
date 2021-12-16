using MQTTnet.Client;
using Rido.IoTClient.AzIoTHub;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rido.IoTClient.IntegrationTests
{
    public class DeviceInfo
    {
        public string UserName { get; set; } = string.Empty;
        public DateTime Started { get; set; } = DateTime.Now;
        public string MachineName { get; set; } = Environment.MachineName;
    }

    

    public class TestPnPClient : PnPClient
    {
        public readonly ReadOnlyProperty<DeviceInfo> Property_deviceInfo;
        private TestPnPClient(IMqttClient connection) : base(connection)
        {
            Property_deviceInfo = new ReadOnlyProperty<DeviceInfo>(connection, "deviceInfo");
        }
        
        public static async Task<TestPnPClient> CreateAsync(ConnectionSettings cs)
        {
            var client = await PnPClient.CreateAsync(cs);
            return new TestPnPClient(client.Connection);
        }
    }
}
