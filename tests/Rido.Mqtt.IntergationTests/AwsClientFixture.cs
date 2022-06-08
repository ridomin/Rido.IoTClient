using Rido.Mqtt.AwsClient;
using Rido.MqttCore;
using System.Threading.Tasks;
using Xunit;

namespace Rido.Mqtt.IntergationTests
{
    public class AwsClientFixture
    {
        private readonly ConnectionSettings cs = new()
        {
            HostName = "a38jrw6jte2l2x-ats.iot.us-west-1.amazonaws.com",
            Auth = AuthType.X509,
            ClientId = "testdevice22",
            X509Key = "testdevice22.pfx|1234"
        };

        //[Fact]
        //public async Task GetShadow()
        //{
        //    var mqtt = await new MqttNet3Adapter.MqttNetClientConnectionFactory().CreateAwsClientAsync(cs);
        //    var client = new Rido.Mqtt.AwsClient.AwsMqttClient(mqtt);
        //    Assert.True(mqtt.IsConnected);
        //    var shadow = await client.GetShadowAsync();
        //    System.Diagnostics.Debug.WriteLine(shadow);
        //    Assert.NotNull(shadow);
        //    await mqtt.DisconnectAsync();
        //    Assert.False(mqtt.IsConnected);
        //}

        [Fact]
        public async Task GetUpdateShadow()
        {
            var mqtt = await new MqttNet3Adapter.MqttNetClientConnectionFactory().CreateAwsClientAsync(cs);
            var client = new AwsMqttClient(mqtt);
            Assert.True(mqtt.IsConnected);
            var shadow = await client.GetShadowAsync();
            Assert.NotNull(shadow);
            var updRes = await client.UpdateShadowAsync(new
            {
                name = "rido3"
            });
            Assert.True(updRes > 0);
            await mqtt.DisconnectAsync();
            Assert.False(mqtt.IsConnected);
        }
    }
}
