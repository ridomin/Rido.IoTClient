using Rido.Mqtt.HubClient;
using System.Threading.Tasks;
using Xunit;

namespace Rido.Mqtt.IntergationTests
{
    public class HubMqttClientFixture
    {
        [Fact]
        public async Task ConnectWithCert()
        {
            var hub = await new MqttNet4Adapter.MqttNetClientConnectionFactory().CreateHubClientAsync("HostName=tests.azure-devices.net;Auth=X509;X509Key=test-device-01.pfx|1234");
            Assert.True(hub.IsConnected);
        }

        [Fact]
        public async Task ConnectWithSas()
        {
            var hub = await new MqttNet4Adapter.MqttNetClientConnectionFactory().CreateHubClientAsync("HostName=tests.azure-devices.net;DeviceId=d8;SharedAccessKey=MDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDA=");
            Assert.True(hub.IsConnected);
        }

        [Fact]
        public async Task ConnectDisconnectWithSas()
        {
            var connection = await new MqttNet4Adapter.MqttNetClientConnectionFactory().CreateHubClientAsync("HostName=tests.azure-devices.net;DeviceId=d8;SharedAccessKey=MDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDA=");
            Assert.True(connection.IsConnected);
            var hubClient = new HubMqttClient(connection);
            await Task.Delay(100);
            bool disconnectCalled = false;

            hubClient.Connection.OnMqttClientDisconnected += (object? sender, MqttCore.DisconnectEventArgs e) =>
            {
                disconnectCalled = true;
            };
            await hubClient.Connection.DisconnectAsync();
            Assert.True(disconnectCalled);
        }

        
    }
}
