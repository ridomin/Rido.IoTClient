using MQTTnet.Client;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient
{
    public interface ITelemetry<T>
    {
        Task<MqttClientPublishResult> SendTelemetryAsync(T payload, CancellationToken cancellationToken = default);
    }
}