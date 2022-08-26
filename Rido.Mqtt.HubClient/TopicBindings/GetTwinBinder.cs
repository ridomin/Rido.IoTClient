using Rido.MqttCore;
using Rido.MqttCore.PnP;

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
        private readonly IMqttConnection connection;

        internal int lastRid = -1;

        public GetTwinBinder(IMqttConnection conn)
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
            lastRid = rid; // for testing
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            var puback = await connection.PublishAsync($"$iothub/twin/GET/?$rid={rid}", string.Empty, 1, false, cancellationToken);

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
