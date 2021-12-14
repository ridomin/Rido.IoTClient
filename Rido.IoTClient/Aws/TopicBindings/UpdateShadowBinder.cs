using MQTTnet.Client;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Aws.TopicBindings
{
    public class UpdateShadowBinder
    {
        readonly static TaskCompletionSource<string> pendingRequest =  new TaskCompletionSource<string>();
        readonly IMqttClient connection;
        readonly string deviceId;
        public UpdateShadowBinder(IMqttClient connection, string deviceId)
        {
            this.deviceId = deviceId;
            this.connection = connection;
            _ = connection.SubscribeAsync($"$aws/things/{deviceId}/shadow/update/accepted");
            connection.ApplicationMessageReceivedAsync += async m =>
            {
                var topic = m.ApplicationMessage.Topic;
                if (topic.StartsWith($"$aws/things/{deviceId}/shadow/update/accepted"))
                {
                    string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                    pendingRequest.SetResult(msg);
                }
                await Task.Yield();
            };
        }

        public async Task<string> UpdateShadowAsync(object payload, CancellationToken cancellationToken = default)
        { 
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            var puback = await connection.PublishAsync($"$aws/things/{deviceId}/shadow/update", payload, cancellationToken);
            if (puback.ReasonCode != MqttClientPublishReasonCode.Success)
            {
                Trace.TraceError("Error publishing message: " + puback.ReasonString);
                throw new ApplicationException(puback.ReasonString);
            }
            return await tcs.Task.TimeoutAfter(TimeSpan.FromSeconds(10));
        }
    }
}
