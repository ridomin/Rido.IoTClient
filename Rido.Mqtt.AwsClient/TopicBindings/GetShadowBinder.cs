using Rido.MqttCore;
using Rido.MqttCore.PnP;

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.AwsClient.TopicBindings
{
    public class GetShadowBinder : IPropertyStoreReader
    {
        private readonly ConcurrentQueue<TaskCompletionSource<string>> pendingGetShadowRequests;
        private readonly IMqttConnection connection;
        private readonly string topicBase;

        public GetShadowBinder(IMqttConnection conn)
        {
            connection = conn;
            pendingGetShadowRequests = new ConcurrentQueue<TaskCompletionSource<string>>();
            string deviceId = conn.ClientId;
            topicBase = $"$aws/things/{deviceId}/shadow";
            _ = connection.SubscribeAsync(topicBase + "/get/accepted");
            connection.OnMessage += async m =>
            {
                var topic = m.Topic;

                if (topic.StartsWith(topicBase + "/get/accepted"))
                {
                    string msg = m.Payload;
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
            await connection.PublishAsync(topicBase + "/get", string.Empty, 1, false, cancellationToken);
            return await pendingGetShadowRequest.Task.TimeoutAfter(TimeSpan.FromSeconds(5));
        }
    }
}
