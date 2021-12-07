using MQTTnet.Client;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub.TopicBindings
{
    public class Telemetry<T>
    {
        readonly IMqttClient connection;
        readonly string deviceId;
        readonly string moduleId;
        readonly string name;

        public Telemetry(IMqttClient connection,  string name, string moduleId = "")
        {
            this.connection = connection;
            this.name = name;
            this.deviceId = connection.Options.ClientId;
            this.moduleId = moduleId;
        }
        public async Task<MqttClientPublishResult> SendTelemetryAsync(T payload, CancellationToken cancellationToken = default) =>
            await SendTelemetryAsync(payload, name, string.Empty, cancellationToken);

        public async Task<MqttClientPublishResult> SendTelemetryAsync(T payload, string name, string componentName = "", CancellationToken cancellationToken = default)
        {
            string topic = $"devices/{deviceId}";

            if (!string.IsNullOrEmpty(moduleId))
            {
                topic += $"/modules/{moduleId}";
            }
            topic += "/messages/events/";

            if (!string.IsNullOrEmpty(componentName))
            {
                topic += $"$.sub={componentName}";
            }

            Dictionary<string, T> typedPayload = new Dictionary<string, T>
            {
                { name, payload }
            };
            return await connection.PublishAsync(topic, typedPayload, cancellationToken);
        }
    }
}