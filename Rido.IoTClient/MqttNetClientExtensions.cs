using MQTTnet;
using MQTTnet.Client;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient
{
    public static class PubSubExtensions
    {
        public static async Task<MqttClientPublishResult> PublishAsync(this IMqttClient client, string topic, object payload, CancellationToken cancellation = default)
        {
            string jsonPayload;

            if (payload is string)
            {
                jsonPayload = payload as string;
            }
            else
            {
                jsonPayload = JsonSerializer.Serialize(payload);
            }

            var message = new MqttApplicationMessageBuilder()
                              .WithTopic(topic)
                              .WithPayload(jsonPayload)
                              .Build();

            var pubAck = await client.PublishAsync(message, cancellation);
            if (pubAck.ReasonCode != MqttClientPublishReasonCode.Success)
            {
                Trace.TraceError(pubAck.ReasonString);
                throw new ApplicationException("Error publishing: " + pubAck.ReasonString);
            }
            return pubAck;
        }
    }
}
