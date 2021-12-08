using Rido.IoTClient.Hive.TopicBindings;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.Tests.Hive
{
    public class TelemetryBinderFixture
    {
        [Fact]
        public async Task SendTelemetry()
        {
            var mqttClient = new MockMqttClient();
            var telemetryBinder = new Telemetry<int>(mqttClient, "temp");
            await telemetryBinder.SendTelemetryAsync(2);
            Assert.Equal("pnp/mock/telemetry", mqttClient.topicRecceived);
            Assert.Equal("{\"temp\":2}", mqttClient.payloadReceived);
        }

        [Fact]
        public async Task SendTelemetryWithComponent()
        {
            var mqttClient = new MockMqttClient();
            var telemetryBinder = new Telemetry<int>(mqttClient, "temp", "myComp");
            await telemetryBinder.SendTelemetryAsync(2);
            Assert.Equal("pnp/mock/myComp/telemetry", mqttClient.topicRecceived);
            Assert.Equal("{\"temp\":2}", mqttClient.payloadReceived);
        }
    }
}
