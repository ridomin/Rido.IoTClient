using Rido.MqttCore;
using System.Threading.Tasks;
using Xunit;

namespace Rido.Mqtt.IntegrationTests
{
    public class HiveConnectionFixture
    {
        private readonly string hostname = "f8826e3352314ca98102cfbde8aff20e.s2.eu.hivemq.cloud";
        private readonly string userName = "client1";
        private readonly string password = "Myclientpwd.000";

        [Fact]
        public async Task ConnectToHive()
        {
            var hiveClient = await new MqttNet4Adapter.MqttNetClientConnectionFactory().CreateBasicClientAsync(new ConnectionSettings
            {
                HostName = hostname,
                UserName = userName,
                Password = password,
                ClientId = "testClientId"

            });
            Assert.True(hiveClient.IsConnected);
        }
    }
}
