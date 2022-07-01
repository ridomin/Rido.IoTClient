using System.Threading.Tasks;
using Xunit;

namespace Rido.Mqtt.UnitTests
{
    public class PublishExtensionFixture
    {
        [Fact]
        public async Task PublishObjectSkipsString()
        {
            var mqttClient = new MockMqttClient();
            await mqttClient.PublishAsync("topic", "{json : true}");
            Assert.Equal("{json : true}", mqttClient.payloadReceived);
        }

        [Fact]
        public async Task PublishSerializeObject()
        {
            var mqttClient = new MockMqttClient();
            await mqttClient.PublishAsync("topic", new { json = true });
            Assert.Equal("{\"json\":true}", mqttClient.payloadReceived);
        }
    }
}
