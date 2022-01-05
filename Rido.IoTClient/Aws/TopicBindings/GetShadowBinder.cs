using MQTTnet.Client;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Aws.TopicBindings
{
    public class GetShadowBinder : IPropertyStoreReader
    {
        readonly ConcurrentQueue<TaskCompletionSource<string>> pendingGetShadowRequests;
        readonly IMqttClient connection;
        readonly string topicBase;

        public GetShadowBinder(IMqttClient conn)
        {
            connection = conn;
            pendingGetShadowRequests = new ConcurrentQueue<TaskCompletionSource<string>>();
            string deviceId = conn.Options.ClientId;
            topicBase = $"$aws/things/{deviceId}/shadow";
            connection.SubscribeAsync(topicBase + "/get/accepted");
            connection.ApplicationMessageReceivedAsync += async m =>
            {
                var topic = m.ApplicationMessage.Topic;

                if (topic.StartsWith(topicBase + "/get/accepted")) 
                {
                    string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                    if (pendingGetShadowRequests.TryDequeue(out var pendingGetShadowRequest))
                    {
                        pendingGetShadowRequest.SetResult(msg);
                    }
                }
                await Task.Yield();
            };
        }

        public async Task<string> ReadPropertiesDocAsync(CancellationToken cancellationToken = default)
        {
            var pendingGetShadowRequest = new TaskCompletionSource<string>();
            pendingGetShadowRequests.Enqueue(pendingGetShadowRequest);
            await connection.PublishAsync(topicBase + "/get", string.Empty, cancellationToken);
            return await pendingGetShadowRequest.Task.TimeoutAfter(TimeSpan.FromSeconds(5));
        }
    }
}
