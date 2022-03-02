
using Rido.MqttCore;
using System;

using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient.TopicBindings
{
    public class Command<T, TResponse> : ICommand<T, TResponse> where T : IBaseCommandRequest<T>, new()
        where TResponse : BaseCommandResponse
    {
        public Func<T, Task<TResponse>> OnCmdDelegate { get; set; }
        public Command(IMqttBaseClient connection, string commandName, string componentName = "")
        {
            var fullCommandName = string.IsNullOrEmpty(componentName) ? commandName : $"{componentName}*{commandName}";
            _ = connection.SubscribeAsync($"$iothub/methods/POST/#");
            connection.OnMessage += async m =>
            {
                var topic = m.Topic;
                if (topic.StartsWith($"$iothub/methods/POST/{fullCommandName}"))
                {
                    string msg = m.Payload;
                    T req = new T().DeserializeBody(msg);
                    if (OnCmdDelegate != null && req != null)
                    {
                        (int rid, _) = TopicParser.ParseTopic(topic);
                        TResponse response = await OnCmdDelegate.Invoke(req);
                        _ = connection.PublishAsync($"$iothub/methods/res/{response.Status}/?$rid={rid}", response);
                    }
                }
            };
        }
    }
}
