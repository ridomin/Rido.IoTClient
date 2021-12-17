using MQTTnet.Client;
using Rido.IoTClient.AzIoTHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.Tests.AzIoTHub
{
    class StubClient : PnPClient 
    {
        public StubClient(IMqttClient c) : base(c){}
    }

    public class TestPnPClientFixture
    {
        static string Stringify(object o) => System.Text.Json.JsonSerializer.Serialize(o);

        MockMqttClient connection;

        public TestPnPClientFixture()
        {
            connection = new MockMqttClient();
        }

        [Fact]
        public void DefaultBindersSet2Subscriptions()
        {
            var client = new StubClient(connection);
            Assert.NotNull(client);
            Assert.Equal(2, connection.numSubscriptions);
            var list = connection.GetInvocationList();
            Assert.Equal(2, list.Count());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.GetTwinBinder", list[0].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.UpdateTwinBinder", list[1].Method.DeclaringType?.FullName?.ToString());
        }

        [Fact]
        public void CountSubscriptionsAndDelegates()
        {
            var client = new TestPnPClient(connection);
            Assert.NotNull(client);
            Assert.Equal(6, connection.numSubscriptions);
            var list = connection.GetInvocationList();
            Assert.Equal(6, list.Count());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.GetTwinBinder", list[0].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.UpdateTwinBinder", list[1].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.DesiredUpdatePropertyBinder", list[2].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.DesiredUpdatePropertyBinder", list[3].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.Command", list[4].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.Command", list[5].Method.DeclaringType?.FullName?.ToString());
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

        [Fact]
        public void ReceiveCommands()
        {
            var client = new TestPnPClient(connection);

            bool receivedWalk = false;
            client.Command_walk.OnCmdDelegate += async m =>
            {
                receivedWalk = true;
                return await Task.FromResult(new EmptyCommandResponse());
            };
            connection.SimulateNewMessage("$iothub/methods/POST/walk", string.Empty);
            Assert.True(receivedWalk);


            bool receivedRun = false;
            client.Command_run.OnCmdDelegate += async m =>
            {
                receivedRun = true;
                return await Task.FromResult(new EmptyCommandResponse());
            };
            connection.SimulateNewMessage("$iothub/methods/POST/run", string.Empty);
            Assert.True(receivedRun);
        }
    }
}
