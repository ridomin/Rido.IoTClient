using MQTTnet.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Aws.TopicBindings
{
    public class UpdateShadowBinder
    {
        TaskCompletionSource<string> pendingRequest;
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
                    pendingRequest.SetResult(msg);
                }
                if (topic.StartsWith($"$aws/things/{deviceId}/shadow/update/rejected"))
                {
                    string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                    pendingRequest.SetException(new ApplicationException(msg));
                }
                await Task.Yield();
            };
        }

        public async Task<string> UpdateShadowAsync(object payload, CancellationToken cancellationToken = default)
        {
            Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>>
            {
                { "state", new Dictionary<string, object>() }
            };
            data["state"].Add("desired", payload);
            

            pendingRequest = new TaskCompletionSource<string>();
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
