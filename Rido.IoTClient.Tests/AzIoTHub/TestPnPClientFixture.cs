using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.Tests.AzIoTHub
{
    public class TestPnPClientFixture
    {
        static string Stringify(object o) => System.Text.Json.JsonSerializer.Serialize(o);

        MockMqttClient connection;

        public TestPnPClientFixture()
        {
            connection = new MockMqttClient();
        }

        [Fact]
        public async void ReportReadOnlyProperty()
        {
            var client = new TestPnPClient(connection);

            var updateTask = client.Property_deviceInfo.UpdateTwinPropertyAsync(
                new DeviceInfo() { UserName = client.Connection.Options.Credentials.Username }
            );

            connection.SimulateNewMessage($"$iothub/twin/res/204/?$rid={RidCounter.Current}&$version={3}", "");
            await Task.Delay(10);
            Assert.True(updateTask.IsCompleted);
            Assert.StartsWith("$iothub/twin/PATCH/properties/reported/?$rid=", connection.topicRecceived);
            Assert.Equal(Stringify(new
            {
                deviceInfo = new
                {
                    UserName = "mockUser",
                    Started = DateTime.MinValue,
                    MachineName = Environment.MachineName
                }
            }), connection.payloadReceived);

            Assert.Equal(Environment.MachineName, client.Property_deviceInfo.PropertyValue.MachineName);
            Assert.StartsWith("mockUser", client.Property_deviceInfo.PropertyValue.UserName);
        }

        [Fact]
        public void ReceiveWritableProperty()
        {
            var client = new TestPnPClient(connection);
            bool received = false;
            client.Property_deviceDesiredState.OnProperty_Updated += async p =>
            {
                received = true;
                await Task.Yield();
                p.Status = 200;
                client.Property_deviceDesiredState.PropertyValue = p;
                return p;
            };
            string twin = Stringify(new
            {
                desiredState = new DesiredDeviceState
                {
                    commandsEnabled = true,
                    telemetryEnabled = false,
                    telemetryInterval = 3
                }
            });
            Assert.Null(client.Property_deviceInfo.PropertyValue);

            connection.SimulateNewMessage("$iothub/twin/PATCH/properties/desired", twin);

            Assert.True(received);
            Assert.Equal(3, client.Property_deviceDesiredState.PropertyValue.Value.telemetryInterval);
            Assert.True(client.Property_deviceDesiredState.PropertyValue.Value.commandsEnabled);
            Assert.False(client.Property_deviceDesiredState.PropertyValue.Value.telemetryEnabled);
        }
    }
}
