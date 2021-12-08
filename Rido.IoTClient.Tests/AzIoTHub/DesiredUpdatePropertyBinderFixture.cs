using Rido.IoTClient.AzIoTHub.TopicBindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.Tests.AzIoTHub
{
    public class DesiredUpdatePropertyBinderFixture
    {
        static string Stringify(object o) => System.Text.Json.JsonSerializer.Serialize(o);
        [Fact]
        public void ReceiveDesired()
        {
            var mqttClient = new MockMqttClient();
            var desiredBinder = new DesiredUpdatePropertyBinder<int>(mqttClient, "myProp");

            desiredBinder.OnProperty_Updated = async p =>
            {
                p.Status = 222;
                return await Task.FromResult(p);
            };

            var desiredMsg = new Dictionary<string, object>();
            desiredMsg.Add("myProp", 1);
            desiredMsg.Add("$version", 3);

            

            mqttClient.SimulateNewMessage("$iothub/twin/PATCH/properties/desired", Stringify(desiredMsg));
            Assert.StartsWith($"$iothub/twin/PATCH/properties/reported/?$rid=", mqttClient.topicRecceived);
            
            var expected = Stringify(new 
            {
                myProp = new
                {
                    av = 3,
                    ac = 222,
                    value = 1,
                }
            });
            Assert.Equal(expected, mqttClient.payloadReceived);
        }
    }
}
