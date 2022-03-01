using Rido.MqttCore;
using System;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient.TopicBindings
{
    public class GenericDesiredUpdatePropertyBinder
    {
        public Func<JsonNode, Task<GenericPropertyAck>> OnProperty_Updated = null;
        public GenericDesiredUpdatePropertyBinder(IMqttBaseClient connection)
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

                     if (desired != null)
                     {
                         if (OnProperty_Updated != null)
                         {
                             var ack = await OnProperty_Updated(desired);
                             if (ack != null)
                             {
                                 _ = updateTwin.ReportPropertyAsync(ack.BuildAck());
                             }
                         }
                     }
                 }
             };
        }


    }
}
