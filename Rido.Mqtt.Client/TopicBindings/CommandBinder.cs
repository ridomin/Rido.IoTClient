using Rido.MqttCore;
using Rido.MqttCore.PnP;

using System;
using System.Text;
using System.Threading.Tasks;

namespace Rido.Mqtt.Client.TopicBindings
{
    public class Command<T, TResponse> : ICommand<T, TResponse>
        where T : IBaseCommandRequest<T>, new()
        where TResponse : BaseCommandResponse
    {
        public Func<T, Task<TResponse>> OnCmdDelegate { get; set; }

        public Command(IMqttBaseClient connection, string commandName, string componentName = "")
        {
            _ = connection.SubscribeAsync($"pnp/+/commands/#");
            connection.OnMessage += async m =>
            {
                var topic = m.Topic;

                var fullCommandName = string.IsNullOrEmpty(componentName) ? commandName : $"{componentName}*{commandName}";

                if (topic.Equals($"pnp/{connection.ClientId}/commands/{fullCommandName}"))
                {
                    T req = new T().DeserializeBody(m.Payload);
                    if (OnCmdDelegate != null && req != null)
                    {
                        TResponse response = await OnCmdDelegate.Invoke(req);
                        _ = connection.PublishAsync($"pnp/{connection.ClientId}/commands/{fullCommandName}/resp/{response.Status}", response);
                    }
                }
            };
        }
    }
}
