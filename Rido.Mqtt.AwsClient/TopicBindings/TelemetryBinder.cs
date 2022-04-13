using MQTTnet;
using MQTTnet.Client;
using Rido.MqttCore;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.AwsClient.TopicBindings
{
    public class Telemetry<T> : ITelemetry<T>
    {
        readonly IMqttBaseClient connection;
        readonly string deviceId;
        readonly string moduleId;
        readonly string name;
        readonly string component;

        public Telemetry(IMqttBaseClient connection, string name, string component = "", string moduleId = "")
        {
            this.connection = connection;
            this.name = name;
            this.component = component;
            deviceId = connection.ClientId;
            this.moduleId = moduleId;
        }
        //public async Task<MqttClientPublishResult> SendTelemetryAsync(T payload, CancellationToken cancellationToken = default) =>
        //    await SendTelemetryAsync(payload, name, string.Empty, cancellationToken);

        public async Task<int> SendTelemetryAsync(T payload, CancellationToken cancellationToken = default)
        {
            string topic = $"pnp/{deviceId}";

            if (!string.IsNullOrEmpty(component))
            {
                topic += $"/{component}";
            }
            if (!string.IsNullOrEmpty(moduleId))
            {
                topic += $"/modules/{moduleId}";
            }
            topic += "/telemetry";


            Dictionary<string, T> typedPayload = new Dictionary<string, T>
            {
                { name, payload }
            };
            return await connection.PublishAsync(topic, typedPayload, 1, false, cancellationToken);
        }
    }
}
