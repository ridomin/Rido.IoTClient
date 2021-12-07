using MQTTnet;
using MQTTnet.Client;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient
{
    public static class PublishExtensions
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

            return await client.PublishAsync(message, cancellation);
        }

    }
}
