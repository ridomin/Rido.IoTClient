using Rido.IoTClient.Hive.TopicBindings;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.Tests.Hive
{
    public class UpdatePropertyBinderFixture
    {
        [Fact]
        public async Task UpdateProp()
        {
            var mqttClient = new MockMqttClient();
            var updateProp = new UpdatePropertyBinder(mqttClient);
            await updateProp.ReportProperty(new { myProp = 1 });
            Assert.Equal("pnp/mock/props/reported", mqttClient.topicRecceived);
            Assert.Equal("{\"myProp\":1}", mqttClient.payloadReceived);
        }
    }
}
