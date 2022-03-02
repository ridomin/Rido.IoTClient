using Rido.MqttCore;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient.TopicBindings
{
    public class GetTwinBinder : IPropertyStoreReader
    {
        private static readonly ConcurrentDictionary<int, TaskCompletionSource<string>> pendingGetTwinRequests = new ConcurrentDictionary<int, TaskCompletionSource<string>>();
        private readonly IMqttBaseClient connection;

        public GetTwinBinder(IMqttBaseClient conn)
        {
            connection = conn;
            _ = connection.SubscribeAsync("$iothub/twin/res/#");
            connection.OnMessage += async m =>
            {
                var topic = m.Topic;

                if (topic.StartsWith("$iothub/twin/res/200"))
                {
                    string msg = m.Payload;
                    (int rid, _) = TopicParser.ParseTopic(topic);
                    if (pendingGetTwinRequests.TryRemove(rid, out var tcs))
                    {
                        tcs.SetResult(msg);
                    }
                }
                await Task.Yield();
            };
        }

        public async Task<string> ReadPropertiesDocAsync(CancellationToken cancellationToken = default)
        {
            var rid = RidCounter.NextValue();
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            var puback = await connection.PublishAsync($"$iothub/twin/GET/?$rid={rid}", string.Empty, 0, cancellationToken);

            if (puback == 0)
            {
                pendingGetTwinRequests.TryAdd(rid, tcs);
            }
            else
            {
                Trace.TraceError($"Error '{puback}' publishing twin GET");
            }
            return await tcs.Task.TimeoutAfter(TimeSpan.FromSeconds(10));
        }

    }
}
