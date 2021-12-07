using MQTTnet.Client;
using System;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub.TopicBindings
{
    public interface IBaseCommandRequest
    {
        public object DeserializeBody(string payload);
    }

    public abstract class BaseCommandResponse
    {
        [JsonIgnore]
        public int Status { get; set; }
    }

    public class CommandBinder<T, TResponse>
        where T : IBaseCommandRequest, new()
        where TResponse : BaseCommandResponse
    {
        public Func<T, Task<TResponse>> OnCmdDelegate { get; set; }

        public CommandBinder(IMqttClient connection, string commandName, string componentName = "")
        {
            _ = connection.SubscribeAsync("$iothub/methods/POST/#");
            connection.ApplicationMessageReceivedAsync += async m =>
            {
                var topic = m.ApplicationMessage.Topic;

                var fullCommandName = string.IsNullOrEmpty(componentName) ? commandName : $"{componentName}*{commandName}";

                if (topic.StartsWith($"$iothub/methods/POST/{fullCommandName}"))
                {
                    string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                    T req = (T)new T().DeserializeBody(msg);
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
