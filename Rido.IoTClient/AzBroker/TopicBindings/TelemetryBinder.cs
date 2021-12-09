using MQTTnet.Client;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzBroker.TopicBindings
{
    public class Telemetry<T>
    {
        readonly IMqttClient connection;
        //readonly string deviceId;
        //readonly string moduleId;
        readonly string name;
        readonly string componentName;

        public Telemetry(IMqttClient connection, string name, string componentName = "") //, string moduleId = "")
        {
            this.connection = connection;
            this.name = name;
            this.componentName = componentName;
            //this.deviceId = connection.Options.ClientId;
            //this.moduleId = moduleId;
        }

        public async Task<MqttClientPublishResult> SendTelemetryAsync(T payload, CancellationToken cancellationToken = default)
        {
            string topic = $"$az/iot/telemetry";

            if (!string.IsNullOrEmpty(componentName))
            {
                topic += $"/?dts={componentName}";
            }

            Dictionary<string, T> typedPayload = new Dictionary<string, T>
            {
                { name, payload }
            };
            return await connection.PublishAsync(topic, typedPayload, cancellationToken);
        }
    }
}