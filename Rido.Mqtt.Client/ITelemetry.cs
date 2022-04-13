using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.Client
{
    public interface ITelemetry<T>
    {
        Task<int> SendTelemetryAsync(T payload, CancellationToken cancellationToken = default);
    }
}