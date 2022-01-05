using MQTTnet.Client;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub.TopicBindings
{
    public class UpdateTwinBinder : IReportPropertyBinder, IPropertyStoreWriter
    {
        readonly static ConcurrentDictionary<int, TaskCompletionSource<int>> pendingRequests = new ConcurrentDictionary<int, TaskCompletionSource<int>>();
        readonly IMqttClient connection;
        
        public UpdateTwinBinder(IMqttClient connection)
        {
            this.connection = connection;
            _ = connection.SubscribeAsync("$iothub/twin/res/#");
            connection.ApplicationMessageReceivedAsync += async m =>
            {
                var topic = m.ApplicationMessage.Topic;
                if (topic.StartsWith("$iothub/twin/res/204"))
                {
                    (int rid, int twinVersion) = TopicParser.ParseTopic(topic);
                    if (pendingRequests.TryRemove(rid, out var tcs))
                    {
                        tcs.SetResult(twinVersion);
                    }
                }
                await Task.Yield();
            };
        }

        public async Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default)
        {
            var rid = RidCounter.NextValue();
            var puback = await connection.PublishAsync($"$iothub/twin/PATCH/properties/reported/?$rid={rid}", payload, cancellationToken);
            var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (puback?.ReasonCode == MqttClientPublishReasonCode.Success)
            {
                pendingRequests.TryAdd(rid, tcs);
            }
            else
            {
                Trace.TraceError($"Error '{puback?.ReasonCode}' publishing twin GET");
            }
            return await tcs.Task.TimeoutAfter(TimeSpan.FromSeconds(10));
        }
    }
}
