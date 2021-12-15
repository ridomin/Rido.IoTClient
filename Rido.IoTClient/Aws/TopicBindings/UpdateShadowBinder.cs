using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;



namespace Rido.IoTClient.Aws.TopicBindings
{
    public class UpdateShadowBinder
    {
        TaskCompletionSource<int> pendingRequest;
        readonly IMqttClient connection;
        readonly string deviceId;
        public UpdateShadowBinder(IMqttClient connection, string deviceId)
        {
            this.deviceId = deviceId;
            this.connection = connection;
            _ = connection.SubscribeAsync($"$aws/things/{deviceId}/shadow/update/+");
            connection.ApplicationMessageReceivedAsync += async m =>
            {
                var topic = m.ApplicationMessage.Topic;
                if (topic.StartsWith($"$aws/things/{deviceId}/shadow/update/accepted"))
                {
                    string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                    JsonNode node = JsonNode.Parse(msg);
                    int version = node["version"].GetValue<int>();
                    if (pendingRequest != null && !pendingRequest.Task.IsCompleted)
                    {
                        pendingRequest.SetResult(version);
                    }
                }
                if (topic.StartsWith($"$aws/things/{deviceId}/shadow/update/rejected"))
                {
                    string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                    if (pendingRequest != null && !pendingRequest.Task.IsCompleted)
                    {
                        pendingRequest.SetException(new ApplicationException(msg));
                    }
                    Trace.TraceWarning(msg);
                }
                await Task.Yield();
            };
        }

        public async Task<int> UpdateShadowAsync(object payload, CancellationToken cancellationToken = default)
        {
            pendingRequest = new TaskCompletionSource<int>();
            Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "state", new Dictionary<string, object>()
                    {
                       { "reported", payload}
                    }
                }
            };
            var puback = await connection.PublishAsync($"$aws/things/{deviceId}/shadow/update", data, cancellationToken);
            if (puback.ReasonCode != MqttClientPublishReasonCode.Success)
            {
                Trace.TraceError("Error publishing message: " + puback.ReasonString);
                throw new ApplicationException(puback.ReasonString);
            }
            return await pendingRequest.Task.TimeoutAfter(TimeSpan.FromSeconds(10));
        }
    }
}
