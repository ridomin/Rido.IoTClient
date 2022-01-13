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
    class StubClient : IoTHubPnPClient
    {
        public StubClient(IMqttClient c) : base(c) { }
    }

    public class TestPnPClientFixture
    {
        static string Stringify(object o) => System.Text.Json.JsonSerializer.Serialize(o);

        readonly MockMqttClient connection;

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
            Assert.Equal(2, list.Length);
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.GetTwinBinder", list[0].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.UpdateTwinBinder", list[1].Method.DeclaringType?.FullName?.ToString());
        }

        [Fact]
        public void CountSubscriptionsAndDelegates()
        {
            var client = new TestPnPClient(connection);
            Assert.NotNull(client);
            Assert.Equal(18, connection.numSubscriptions);
            var list = connection.GetInvocationList();
            Assert.Equal(18, list.Length);
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.GetTwinBinder", list[0].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.UpdateTwinBinder", list[1].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.UpdateTwinBinder", list[2].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.UpdateTwinBinder", list[3].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.UpdateTwinBinder", list[4].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.UpdateTwinBinder", list[5].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.DesiredUpdatePropertyBinder", list[6].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.UpdateTwinBinder", list[7].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.UpdateTwinBinder", list[8].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.DesiredUpdatePropertyBinder", list[9].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.Command", list[10].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.Command", list[11].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.UpdateTwinBinder", list[12].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.UpdateTwinBinder", list[13].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.UpdateTwinBinder", list[14].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.UpdateTwinBinder", list[15].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.DesiredUpdatePropertyBinder", list[16].Method.DeclaringType?.FullName?.ToString());
            Assert.StartsWith("Rido.IoTClient.AzIoTHub.TopicBindings.Command", list[17].Method.DeclaringType?.FullName?.ToString());
        }


        [Fact]
        public async void ReportReadOnlyProperty()
        {
            var client = new TestPnPClient(connection);
            client.Property_deviceInfo.PropertyValue = new DeviceInfo() { UserName = client.Connection.Options.Credentials.Username };
            var updateTask = client.Property_deviceInfo.ReportPropertyAsync();

            connection.SimulateNewMessage($"$iothub/twin/res/204/?$rid={RidCounter.Current}&$version={3}", "");
            await Task.Delay(20);
            Assert.True(updateTask.IsCompleted);
            Assert.StartsWith("$iothub/twin/PATCH/properties/reported/?$rid=", connection.topicRecceived);
            Assert.Equal(Stringify(new
            {
                deviceInfo = new
                {
                    UserName = "mockUser",
                    Started = DateTime.MinValue,
                    Environment.MachineName
                }
            }), connection.payloadReceived);

            Assert.Equal(Environment.MachineName, client.Property_deviceInfo.PropertyValue.MachineName);
            Assert.StartsWith("mockUser", client.Property_deviceInfo.PropertyValue.UserName);
        }

        [Fact]
        public async void ReportWritableProperty()
        {
            var client = new TestPnPClient(connection);

            client.Property_deviceDesiredState.PropertyValue.Version = 0;
            client.Property_deviceDesiredState.PropertyValue.Status = 203;
            client.Property_deviceDesiredState.PropertyValue.Description = "fake description";
            client.Property_deviceDesiredState.PropertyValue.Value = new DesiredDeviceState()
            {
                commandsEnabled = true,
                telemetryEnabled = false,
                telemetryInterval = 123
            };
            var updateTask = client.Property_deviceDesiredState.ReportPropertyAsync();

            connection.SimulateNewMessage($"$iothub/twin/res/204/?$rid={RidCounter.Current}&$version={3}", "");
            //await Task.Delay(20);
            await updateTask.WaitAsync(TimeSpan.FromMilliseconds(100));
            Assert.True(updateTask.IsCompletedSuccessfully, "status " + updateTask.Status.ToString());
            Assert.StartsWith("$iothub/twin/PATCH/properties/reported/?$rid=", connection.topicRecceived);
            Assert.Equal(Stringify(new
            {
                desiredState = new
                {
                    av = 0,
                    ad = "fake description",
                    ac = 203,
                    value = new
                    {
                        telemetryInterval = 123,
                        commandsEnabled = true,
                        telemetryEnabled = false
                    }
                }

            }), connection.payloadReceived);

            Assert.True(client.Property_deviceDesiredState.PropertyValue.Value.commandsEnabled);
            Assert.False(client.Property_deviceDesiredState.PropertyValue.Value.telemetryEnabled);
            Assert.Equal(123, client.Property_deviceDesiredState.PropertyValue.Value.telemetryInterval);
            Assert.Equal(0, client.Property_deviceDesiredState.PropertyValue.Version);
            Assert.Equal(203, client.Property_deviceDesiredState.PropertyValue.Status);
            Assert.Equal("fake description", client.Property_deviceDesiredState.PropertyValue.Description);
        }

        [Fact]
        public async void ReportWritablePropertyInComponent()
        {
            var client = new TestPnPClient(connection);

            client.Component_testInfo.Property_deviceInfo.PropertyValue.Version = 0;
            client.Component_testInfo.Property_deviceInfo.PropertyValue.Status = 203;
            client.Component_testInfo.Property_deviceInfo.PropertyValue.Description = "fake description";
            client.Component_testInfo.Property_deviceInfo.PropertyValue.Value = new DeviceInfo()
            {
                MachineName = Environment.MachineName,
                Started = DateTime.MinValue,
                UserName = connection.Options.Credentials.Username
            };
            var updateTask = client.Component_testInfo.Property_deviceInfo.ReportPropertyAsync();

            connection.SimulateNewMessage($"$iothub/twin/res/204/?$rid={RidCounter.Current}&$version={3}", "");
            await Task.Delay(20);
            Assert.True(updateTask.IsCompleted);
            Assert.StartsWith("$iothub/twin/PATCH/properties/reported/?$rid=", connection.topicRecceived);
            Assert.Equal(Stringify(new
            {
                testInfo = new
                {
                    __t = "c",
                    deviceInfo = new
                    {
                        av = 0,
                        ad = "fake description",
                        ac = 203,
                        value = new
                        {
                            UserName = "mockUser",
                            Started = DateTime.MinValue,
                            Environment.MachineName
                        }
                    }
                }
            }), connection.payloadReceived);

            Assert.Equal(Environment.MachineName, client.Component_testInfo.Property_deviceInfo.PropertyValue.Value.MachineName);
            Assert.Equal(DateTime.MinValue, client.Component_testInfo.Property_deviceInfo.PropertyValue.Value.Started);
            Assert.Equal("mockUser", client.Component_testInfo.Property_deviceInfo.PropertyValue.Value.UserName);
            Assert.Equal(0, client.Component_testInfo.Property_deviceInfo.PropertyValue.Version);
            Assert.Equal(203, client.Component_testInfo.Property_deviceInfo.PropertyValue.Status);
            Assert.Equal("fake description", client.Component_testInfo.Property_deviceInfo.PropertyValue.Description);
        }


        [Fact]
        public async void ReportOneReadOnlyPropertyInComponent()
        {
            var client = new TestPnPClient(connection);

            client.Component_testInfo.Property_name.PropertyValue = "testName";
            var updateTask = client.Component_testInfo.Property_name.ReportPropertyAsync();
            Assert.Equal("testName", client.Component_testInfo.Property_name.PropertyValue);

            connection.SimulateNewMessage($"$iothub/twin/res/204/?$rid={RidCounter.Current}&$version={3}", "");
            await Task.Delay(20);
            Assert.True(updateTask.IsCompleted);
            Assert.StartsWith("$iothub/twin/PATCH/properties/reported/?$rid=", connection.topicRecceived);
            Assert.Equal(Stringify(new
            {
                testInfo = new
                {
                    __t = "c",
                    name = "testName",
                }
            }), connection.payloadReceived);
        }

        [Fact]
        public async void ReportAllReadOnlyPropertyInComponent()
        {
            var client = new TestPnPClient(connection);
            client.Component_testInfo.Property_name.PropertyValue = "testName";
            var updateTask = client.Component_testInfo.ReportPropertyAsync();
            Assert.Equal("testName", client.Component_testInfo.Property_name.PropertyValue);

            connection.SimulateNewMessage($"$iothub/twin/res/204/?$rid={RidCounter.Current}&$version={3}", "");
            await Task.Delay(20);
            Assert.True(updateTask.IsCompleted);
            Assert.StartsWith("$iothub/twin/PATCH/properties/reported/?$rid=", connection.topicRecceived);
            Assert.Equal(Stringify(new
            {
                testInfo = new
                {
                    name = "testName",
                    __t = "c",
                }
            }), connection.payloadReceived);
        }

        [Fact]
        public async Task ReceiveWritableProperty()
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
            await Task.Delay(10);
            Assert.True(received);
            Assert.Equal(3, client.Property_deviceDesiredState.PropertyValue.Value.telemetryInterval);
            Assert.True(client.Property_deviceDesiredState.PropertyValue.Value.commandsEnabled);
            Assert.False(client.Property_deviceDesiredState.PropertyValue.Value.telemetryEnabled);
        }

        [Fact]
        public async Task ReceiveWritableComponentProperty()
        {
            var client = new TestPnPClient(connection);
            bool received = false;
            client.Component_testInfo.Property_deviceInfo.OnProperty_Updated += async p =>
            {
                received = true;
                await Task.Yield();
                p.Status = 200;
                client.Component_testInfo.Property_deviceInfo.PropertyValue = p;
                return p;
            };
            string twin = Stringify(new
            {
                testInfo = new
                {
                    __t = "c",
                    deviceInfo = new DeviceInfo
                    {
                        MachineName = Environment.MachineName,
                        Started = DateTime.MinValue,
                        UserName = connection.Options.Credentials.Username
                    }
                }
            });
            Assert.Null(client.Component_testInfo.Property_deviceInfo.PropertyValue.Value);
            connection.SimulateNewMessage("$iothub/twin/PATCH/properties/desired", twin);
            await Task.Delay(10);
            Assert.True(received);
            Assert.Equal(Environment.MachineName, client.Component_testInfo.Property_deviceInfo.PropertyValue.Value.MachineName);
            Assert.Equal(DateTime.MinValue, client.Component_testInfo.Property_deviceInfo.PropertyValue.Value.Started);
            Assert.Equal("mockUser", client.Component_testInfo.Property_deviceInfo.PropertyValue.Value.UserName);
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
                return await Task.FromResult(new EmptyCommandResponse() { Status = 200 });
            };
            connection.SimulateNewMessage("$iothub/methods/POST/run", string.Empty);
            Assert.True(receivedRun);

            bool received_start = false;
            client.Component_testInfo.Command_start.OnCmdDelegate += async m =>
            {
                received_start = true;
                return await Task.FromResult(new EmptyCommandResponse() { Status = 200 });
            };
            connection.SimulateNewMessage("$iothub/methods/POST/testInfo*start", string.Empty);
            Assert.True(received_start);
        }
    }
}
