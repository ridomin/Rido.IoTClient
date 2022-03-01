using Rido.MqttCore;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient.TopicBindings
{
    public class Telemetry<T> : ITelemetry<T>
    {
        readonly IMqttBaseClient connection;
        readonly string deviceId;
        readonly string moduleId;
        readonly string name;
        readonly string componentName;

        public Telemetry(IMqttBaseClient connection, string name, string componentName = "", string moduleId = "")
        {
            this.connection = connection;
            this.name = name;
            this.componentName = componentName;
            this.deviceId = connection.ClientId;
            this.moduleId = moduleId;
        }

        public async Task<int> SendTelemetryAsync(T payload, CancellationToken cancellationToken = default)
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

            return await connection.PublishAsync(topic, JsonSerializer.Serialize(typedPayload), 0, cancellationToken);
        }
    }
}