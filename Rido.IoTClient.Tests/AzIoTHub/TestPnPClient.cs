using MQTTnet.Client;
using Rido.IoTClient.AzIoTHub;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rido.IoTClient.Tests.AzIoTHub
{
    public class DeviceInfo
    {
        public string UserName { get; set; } = string.Empty;
        public DateTime Started { get; set; } = DateTime.MinValue;
        public string MachineName { get; set; } = Environment.MachineName;
    }

    public class DesiredDeviceState
    {
        public int telemetryInterval { get; set; }
        public bool commandsEnabled { get; set; }
        public bool telemetryEnabled { get; set; }
    }

    public class TestPnPClient : PnPClient
    {
        public readonly ReadOnlyProperty<DeviceInfo> Property_deviceInfo;
        public readonly WritableProperty<DesiredDeviceState> Property_deviceDesiredState;

        internal TestPnPClient(IMqttClient connection) : base(connection)
        {
            Property_deviceInfo = new ReadOnlyProperty<DeviceInfo>(connection, "deviceInfo");
            Property_deviceDesiredState = new WritableProperty<DesiredDeviceState>(connection, "desiredState");
        }
    }
}
