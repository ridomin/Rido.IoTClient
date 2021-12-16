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

    public class desiredState
    {
        public int telemetryInterval { get; set; }
        public bool commandsEnabled { get; set; }
        public bool telemetryEnabled { get; set; }
    }

    public class TestPnPClient : PnPClient
    {
        public readonly ReadOnlyProperty<DeviceInfo> Property_deviceInfo;
        public readonly WritableProperty<desiredState> Property_deviceDesiredState;
        private TestPnPClient(IMqttClient connection) : base(connection)
        {
            Property_deviceInfo = new ReadOnlyProperty<DeviceInfo>(connection, "deviceInfo");
            Property_deviceDesiredState = new WritableProperty<desiredState>(connection, "desiredState");
        }
        
        public static async Task<TestPnPClient> CreateAsync(ConnectionSettings cs)
        {
            var connection = await IoTHubConnectionFactory.CreateAsync(cs);
            var client = new TestPnPClient(connection) { ConnectionSettings = cs };
            var twin = await client.GetTwinAsync();
            client.InitialTwin = twin;
            return client; 
        }
    }
}
