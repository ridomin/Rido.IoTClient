using System.Threading;
using System.Threading.Tasks;

namespace Rido.MqttCore.PnP
{
    /// <summary>
    /// Updates device properties
    /// </summary>
    public interface IPropertyStoreWriter
    {
        /// <summary>
        /// Writable properties can also be reported from the device
        /// </summary>
        /// <param name="payload">properties payload</param>
        /// <param name="token">Async Cancellation token</param>
        /// <returns></returns>
        Task<int> ReportPropertyAsync(object payload, CancellationToken token = default);
    }
}
