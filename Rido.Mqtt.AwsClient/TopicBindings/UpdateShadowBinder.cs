using Rido.MqttCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.AwsClient.TopicBindings
{
    public class UpdateShadowBinder : IReportPropertyBinder, IPropertyStoreWriter
    {
        readonly ConcurrentQueue<TaskCompletionSource<int>> pendingRequests;
        readonly IMqttBaseClient connection;

        public UpdateShadowBinder(IMqttBaseClient connection)
        {
            this.connection = connection;
            pendingRequests = new ConcurrentQueue<TaskCompletionSource<int>>();
            _ = connection.SubscribeAsync($"$aws/things/{connection.ClientId}/shadow/update/accepted");
            connection.OnMessage += async m =>
            {
                await Task.Yield();
                var topic = m.Topic;
                if (topic.StartsWith($"$aws/things/{connection.ClientId}/shadow/update/accepted"))
                {
                    string msg = m.Payload;
                    JsonNode node = JsonNode.Parse(msg);
                    int version = node["version"].GetValue<int>();
                    if (pendingRequests.TryDequeue(out var pendingRequest))
                    {
                        pendingRequest.SetResult(version);
                    }
                }
            };
        }

        public async Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<int>();
            pendingRequests.Enqueue(tcs);
            Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "state", new Dictionary<string, object>()
                    {
                       { "reported", payload}
                    }
                }
            };
            var puback = await connection.PublishAsync($"$aws/things/{connection.ClientId}/shadow/update", data, 1 , true, cancellationToken);
            if (puback != 0)
            {
                Trace.TraceError("Error publishing message: " + puback);
                throw new ApplicationException("Publishing Exception");
            }
            return await tcs.Task.TimeoutAfter(TimeSpan.FromSeconds(5));
        }
    }
}
