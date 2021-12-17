using MQTTnet.Client;
using System;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.IntegrationTests
{
    public class TestPnPClientFixture
    {
        readonly ConnectionSettings cs = new()
        {
            HostName = "tests.azure-devices.net",
            DeviceId = "d10",
            SharedAccessKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.Empty.ToString("N")))
        };

        [Fact]
        public async Task ValidateReadOnlyProperty()
        {
            var client = await TestPnPClient.CreateAsync(cs);

            await client.Property_deviceInfo.ReportPropertyAsync(
                new DeviceInfo() { UserName = client.Connection.Options.Credentials.Username }
            );

            var twinJson = await client.GetTwinAsync();

            var twin = JsonNode.Parse(twinJson);
            Assert.NotNull(twin);
            Assert.StartsWith(cs.HostName, twin?["reported"]?[client.Property_deviceInfo.Name]?["UserName"]?.GetValue<string>());
            Assert.Equal(Environment.MachineName, client.Property_deviceInfo.PropertyValue.MachineName);
            Assert.StartsWith(cs.HostName, client.Property_deviceInfo.PropertyValue.UserName);

            await client.Connection.DisconnectAsync(MqttClientDisconnectReason.NormalDisconnection);
        }

        [Fact]
        public async Task ValidateWritableProperty()
        {
            var client = await TestPnPClient.CreateAsync(cs);
            bool received = false;
            client.Property_deviceDesiredState.OnProperty_Updated += async p =>
            {
                received = true;
                await Task.Yield();
                p.Status = 200;
                client.Property_deviceDesiredState.PropertyValue = p;
                return p;
            };
            Assert.Null(client.Property_deviceInfo.PropertyValue);
            await client.Property_deviceDesiredState.InitPropertyAsync(client.InitialTwin, new DesiredDeviceState());
            await Task.Delay(100);
            Assert.True(received);
            Assert.Equal(3, client.Property_deviceDesiredState.PropertyValue.Value.telemetryInterval);
        }
    }
}
