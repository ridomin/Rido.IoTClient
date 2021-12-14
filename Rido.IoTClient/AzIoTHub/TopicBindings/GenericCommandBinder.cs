
using MQTTnet.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub.TopicBindings
{
    public class GenericCommand
    {
        public Func<GenericCommandRequest, Task<GenericCommandResponse>> OnCmdDelegate { get; set; }

        public GenericCommand(IMqttClient connection)
        {
            _ = connection.SubscribeAsync("$iothub/methods/POST/#");
            connection.ApplicationMessageReceivedAsync += async m =>
            {
                var topic = m.ApplicationMessage.Topic;
                if (topic.StartsWith($"$iothub/methods/POST/"))
                {
                    var segments = topic.Split('/');
                    var cmdName = segments[3];
                    string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                    GenericCommandRequest req = new GenericCommandRequest()
                    {
                        CommandName = cmdName,
                        CommandPayload = msg
                    };
                    if (OnCmdDelegate != null && req != null)
                    {
                        (int rid, _) = TopicParser.ParseTopic(topic);
                        GenericCommandResponse response = await OnCmdDelegate.Invoke(req);
                        _ = connection.PublishAsync($"$iothub/methods/res/{response.Status}/?$rid={rid}", response);
                    }
                }
            };
        }
    }
}
