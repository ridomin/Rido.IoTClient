using MQTTnet.Client;
using Rido.IoTClient.AzIoTHub;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System;

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

    public class TestPnPClient : IoTHubPnPClient
    {
        public readonly ReadOnlyProperty<int> Property_counter;
        public readonly WritableProperty<string> Property_message;
        public readonly ReadOnlyProperty<DeviceInfo> Property_deviceInfo;
        public readonly WritableProperty<DesiredDeviceState> Property_deviceDesiredState;
        public readonly Command<EmptyCommandRequest, EmptyCommandResponse> Command_run;
        public readonly Command<EmptyCommandRequest, EmptyCommandResponse> Command_walk;

        internal TestPnPClient(IMqttClient connection) : base(connection)
        {
            Property_counter = new ReadOnlyProperty<int>(connection, "counter");
            Property_deviceInfo = new ReadOnlyProperty<DeviceInfo>(connection, "deviceInfo");
            Property_message = new WritableProperty<string>(connection, "message");
            Property_deviceDesiredState = new WritableProperty<DesiredDeviceState>(connection, "desiredState");
            Command_run = new Command<EmptyCommandRequest, EmptyCommandResponse>(connection, "run");
            Command_walk = new Command<EmptyCommandRequest, EmptyCommandResponse>(connection, "walk");
        }
    }
}
