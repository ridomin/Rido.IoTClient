using Rido.MqttCore;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient.TopicBindings
{
    public class Telemetry<T> : ITelemetry<T>
    {
        private readonly IMqttBaseClient connection;
        private readonly string deviceId;
        private readonly string moduleId;
        private readonly string name;
        private readonly string componentName;

        public Telemetry(IMqttBaseClient connection, string name, string componentName = "", string moduleId = "")
        {
            this.connection = connection;
            this.name = name;
            this.componentName = componentName;
            deviceId = connection.ClientId;
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

            return await connection.PublishAsync(topic, typedPayload, 1, false, cancellationToken);
        }
    }
}