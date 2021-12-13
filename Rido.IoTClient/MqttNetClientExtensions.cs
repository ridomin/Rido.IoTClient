using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace Rido.IoTClient
{
    public static class PubSubExtensions
    {
        static readonly HashSet<string> subscribedTopics = new HashSet<string> { };
        public static async Task<MqttClientSubscribeResult> SingleSubscribeAsync(this IMqttClient client, string topic, CancellationToken cancellation = default)
        {
            MqttClientSubscribeResult subAck = new MqttClientSubscribeResult();
            if (!subscribedTopics.Contains(topic))
            {
                subAck = await client.SubscribeAsync(
                    new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(topic)
                    .Build(),
                    cancellation);
                subscribedTopics.Add(topic);
                Trace.TraceInformation("Sub to " + topic);
            }

            if (subAck.Items.Where(x => (int)x.ResultCode > 2).Count() > 0)
            {
                subAck.Items.ForEach(x => Trace.TraceError($"+ {x.TopicFilter.Topic} {x.ResultCode}"));
                throw new ApplicationException($"Error subscribing to `{topic}`");
            }

            return subAck;
        }


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
