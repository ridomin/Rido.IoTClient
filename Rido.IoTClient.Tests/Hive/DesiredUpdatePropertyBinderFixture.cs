﻿using Rido.IoTClient.Hive.TopicBindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.Tests.Hive
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

            mqttClient.SimulateNewMessage("pnp/mock/props/set", Stringify(new { myProp = 1 }));

            Assert.Equal("pnp/mock/props/reported", mqttClient.topicRecceived);
            var expected = Stringify(new 
            {
                myProp = new
                {
                    ac = 222,
                    value = 1
                }
            });
            Assert.Equal(expected, mqttClient.payloadReceived);
        }
    }
}