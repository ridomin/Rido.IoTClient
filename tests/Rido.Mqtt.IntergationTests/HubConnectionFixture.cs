using Rido.MqttCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rido.Mqtt.IntergationTests
{
    public class HubMqttClientFixture
    {
        [Fact]
        public async Task ConnectWithCert()
        {
            var hub = await new MqttNet3Adapter.MqttNetClientConnectionFactory().CreateHubClientAsync("HostName=tests.azure-devices.net;Auth=X509;X509Key=test-device-01.pfx|1234");
            Assert.True(hub.IsConnected);
        }

        [Fact]
        public async Task ConnectWithSas()
        {
            var hub = await new MqttNet3Adapter.MqttNetClientConnectionFactory().CreateHubClientAsync("HostName=tests.azure-devices.net;DeviceId=d8;SharedAccessKey=MDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDA=");
            Assert.True(hub.IsConnected);
        }
    }
}
