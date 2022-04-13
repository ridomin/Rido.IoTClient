using Rido.MqttCore;
using Rido.PnP;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient.TopicBindings
{
    public class DesiredUpdatePropertyBinder<T>
    {
        public Func<PropertyAck<T>, Task<PropertyAck<T>>> OnProperty_Updated = null;
        public DesiredUpdatePropertyBinder(IMqttBaseClient connection, string propertyName, string componentName = "")
        {

            _ = connection.SubscribeAsync("$iothub/twin/PATCH/properties/desired/#");
            IPropertyStoreWriter updateTwin = new UpdateTwinBinder(connection);
            connection.OnMessage += async m =>
             {
                 var topic = m.Topic;
                 if (topic.StartsWith("$iothub/twin/PATCH/properties/desired"))
                 {
                     string msg = m.Payload;
                     JsonNode desired = JsonNode.Parse(msg);
                     JsonNode desiredProperty = TwinParser.ReadPropertyFromDesired(desired, propertyName, componentName);
                     if (desiredProperty != null)
                     {
                         if (OnProperty_Updated != null)
                         {
                             var property = new PropertyAck<T>(propertyName, componentName)
                             {
                                 Value = desiredProperty.Deserialize<T>(),
                                 Version = desired?["$version"]?.GetValue<int>() ?? 0
                             };
                             var ack = await OnProperty_Updated(property);
                             if (ack != null)
                             {
                                 _ = updateTwin.ReportPropertyAsync(ack.ToAckDict());
                             }
                         }
                     }
                 }
             };
        }


    }
}
