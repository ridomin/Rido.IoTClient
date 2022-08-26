using System.Threading;
using System.Threading.Tasks;

namespace Rido.MqttCore.PnP
{
    /// <summary>
    /// ITelemetry
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITelemetry<T>
    {
        /// <summary>
        /// SendTelemetryAsync
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SendTelemetryAsync(T payload, CancellationToken cancellationToken = default);
    }
}