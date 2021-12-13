using MQTTnet.Client;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.Tests.AzIoTHub
{
    public class GetTwinBinderFixture
    {
        readonly MockMqttClient mockClient;
        readonly GetTwinBinder binder;

        public GetTwinBinderFixture()
        {
            mockClient = new MockMqttClient();
            binder = new GetTwinBinder(mockClient);
        }

        [Fact]
        public void GetTwinAsync()
        {
            var twinTask = binder.GetTwinAsync();
            mockClient.SimulateNewMessage($"$iothub/twin/res/200/?$rid={RidCounter.Current}", SampleTwin);
            Assert.Equal($"$iothub/twin/GET/?$rid={RidCounter.Current}", mockClient.topicRecceived);
            var twin = twinTask.Result;
            Assert.Equal(twin, SampleTwin);
        }

        static string Stringify(object o) => System.Text.Json.JsonSerializer.Serialize(o);
        static string SampleTwin
        {
            get => Stringify(new
            {
                reported = new
                {
                    myProp = "myValue"
                },
                desired = new Dictionary<string, object>() { { "$version", 1 } },
            });

        }
    }
}
