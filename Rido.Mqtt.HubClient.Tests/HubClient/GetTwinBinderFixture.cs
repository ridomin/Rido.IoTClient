using Rido.Mqtt.HubClient.TopicBindings;
using System.Collections.Generic;
using Xunit;

namespace Rido.Mqtt.HubClient.Tests.HubClient
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
            var twinTask = binder.ReadPropertiesDocAsync();
            mockClient.SimulateNewMessage($"$iothub/twin/res/200/?$rid={RidCounter.Current}", SampleTwin);
            Assert.StartsWith("$iothub/twin/GET/?$rid=", mockClient.topicRecceived);
            Assert.Equal(string.Empty, mockClient.payloadReceived);
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
