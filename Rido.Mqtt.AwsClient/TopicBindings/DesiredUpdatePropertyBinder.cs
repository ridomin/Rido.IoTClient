
using Rido.MqttCore;
using Rido.MqttCore.PnP;

using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Rido.Mqtt.AwsClient.TopicBindings
{
    public class DesiredUpdatePropertyBinder<T>
    {
        public Func<PropertyAck<T>, Task<PropertyAck<T>>> OnProperty_Updated = null;
        public DesiredUpdatePropertyBinder(IMqttBaseClient connection, string propertyName, string componentName = "")
        {
            ;
            _ = connection.SubscribeAsync($"$aws/things/{connection.ClientId}/shadow/update/accepted");
            IPropertyStoreWriter updateShadow = new UpdateShadowBinder(connection);
            connection.OnMessage += async m =>
             {
                 var topic = m.Topic;
                 if (topic.StartsWith($"$aws/things/{connection.ClientId}/shadow/update/accepted"))
                 {
                     string msg = m.Payload;
                     JsonNode root = JsonNode.Parse(msg);
                     JsonNode desired = root["state"]["desired"];
                     JsonNode desiredProperty = null;
                     if (string.IsNullOrEmpty(componentName))
                     {
                         desiredProperty = desired?[propertyName];
                     }
                     else
                     {
                         if (desired != null &&
                            desired[componentName] != null &&
                             desired[componentName][propertyName] != null &&
                             desired[componentName]["__t"] != null &&
                             desired[componentName]["__t"].GetValue<string>() == "c")
                         {
                             desiredProperty = desired?[componentName][propertyName];
                         }
                     }

                     if (desiredProperty != null)
                     {
                         if (OnProperty_Updated != null)
                         {
                             var property = new PropertyAck<T>(propertyName, componentName)
                             {
                                 Value = desiredProperty.Deserialize<T>(),
                                 Version = root["version"]?.GetValue<int>() ?? 0
                             };
                             var ack = await OnProperty_Updated(property);
                             if (ack != null)
                             {
                                 _ = updateShadow.ReportPropertyAsync(ack.ToAckDict());
                             }
                         }
                     }
                 }
             };
        }
    }
}
