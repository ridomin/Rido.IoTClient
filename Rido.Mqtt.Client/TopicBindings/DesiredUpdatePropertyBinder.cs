﻿using Rido.MqttCore;
using Rido.MqttCore.PnP;

using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Rido.Mqtt.Client.TopicBindings
{
    public class DesiredUpdatePropertyBinder<T>
    {
        public Func<PropertyAck<T>, Task<PropertyAck<T>>> OnProperty_Updated = null;
        public DesiredUpdatePropertyBinder(IMqttConnection connection, string propertyName, string componentName = "")
        {
            _ = connection.SubscribeAsync($"pnp/{connection.ClientId}/props/#");
            IReportPropertyBinder propertyBinder = new UpdatePropertyBinder(connection, propertyName);
            connection.OnMessage += async m =>
            {
                var topic = m.Topic;
                if (topic.StartsWith($"pnp/{connection.ClientId}/props/{propertyName}/set"))
                {
                    JsonNode desiredProperty = JsonNode.Parse(m.Payload);
                    //JsonNode desiredProperty = PropertyParser.ReadPropertyFromDesired(desired, propertyName, componentName);
                    //var desiredProperty = desired?[propertyName];
                    if (desiredProperty != null)
                    {
                        if (OnProperty_Updated != null)
                        {
                            var property = new PropertyAck<T>(propertyName, componentName)
                            {
                                Value = desiredProperty.Deserialize<T>(),
                                //Version = desired?["$version"]?.GetValue<int>() ?? 0
                            };
                            var ack = await OnProperty_Updated(property);
                            if (ack != null)
                            {
                                //_ = updateTwin.SendRequestWaitForResponse(ack);
                                _ = propertyBinder.ReportPropertyAsync(ack);
                            }
                        }
                    }
                }
            };
        }
    }
}
