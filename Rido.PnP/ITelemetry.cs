using System.Threading;
using System.Threading.Tasks;

namespace Rido.PnP
{
    public interface ITelemetry<T>
    {
        Task<int> SendTelemetryAsync(T payload, CancellationToken cancellationToken = default);
    }
}