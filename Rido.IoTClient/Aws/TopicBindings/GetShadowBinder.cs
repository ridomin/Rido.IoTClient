using MQTTnet.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Aws.TopicBindings
{
    public class GetShadowBinder : IPropertyStoreReader
    {
        TaskCompletionSource<string> pendingGetShadowRequest;
        readonly IMqttClient connection;
        readonly string topicBase;

        public GetShadowBinder(IMqttClient conn)
        {
            connection = conn;
            string deviceId = conn.Options.ClientId;
            topicBase = $"$aws/things/{deviceId}/shadow";
            connection.SubscribeAsync(topicBase + "/get/accepted");
            connection.ApplicationMessageReceivedAsync += async m =>
            {
                var topic = m.ApplicationMessage.Topic;

                if (topic.StartsWith(topicBase + "/get/accepted")) 
                {
                    string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                    if (pendingGetShadowRequest != null)
                    {
                        pendingGetShadowRequest.SetResult(msg);
                    }
                }
                await Task.Yield();
            };
        }

        public async Task<string> ReadPropertiesDocAsync(CancellationToken cancellationToken = default)
        {
            pendingGetShadowRequest = new TaskCompletionSource<string>();
            await connection.PublishAsync(topicBase + "/get", string.Empty, cancellationToken);
            return await pendingGetShadowRequest.Task.TimeoutAfter(TimeSpan.FromSeconds(5));
        }
    }
}
