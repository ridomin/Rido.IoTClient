using Rido.MqttCore;
using Rido.PnP;
using System;
using System.Threading.Tasks;

namespace Rido.Mqtt.AwsClient.TopicBindings
{

    public class Command<T, TResponse> : ICommand<T, TResponse>
        where T : IBaseCommandRequest<T>, new()
        where TResponse : BaseCommandResponse
    {
        public Func<T, Task<TResponse>> OnCmdDelegate { get; set; }

        public Command(IMqttBaseClient connection, string commandName, string componentName = "")
        {
            _ = connection.SubscribeAsync($"pnp/{connection.ClientId}/commands/#");
            connection.OnMessage += async m =>
            {
                var topic = m.Topic;

                var fullCommandName = string.IsNullOrEmpty(componentName) ? commandName : $"{componentName}*{commandName}";

                if (topic.Equals($"pnp/{connection.ClientId}/commands/{fullCommandName}"))
                {
                    string msg = m.Payload;
                    T req = new T().DeserializeBody(msg);
                    if (OnCmdDelegate != null && req != null)
                    {
                        //(int rid, _) = TopicParser.ParseTopic(topic);
                        TResponse response = await OnCmdDelegate.Invoke(req);
                        _ = connection.PublishAsync($"pnp/{connection.ClientId}/commands/{fullCommandName}/resp/{response.Status}", response);
                    }
                }
            };
        }
    }
}
