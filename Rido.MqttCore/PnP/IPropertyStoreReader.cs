using System.Threading;
using System.Threading.Tasks;

namespace Rido.MqttCore.PnP
{
    /// <summary>
    /// Interface to expose device properties
    /// </summary>
    public interface IPropertyStoreReader
    {
        /// <summary>
        /// Returns the device properties as string (like the device twin)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> ReadPropertiesDocAsync(CancellationToken cancellationToken = default);
    }
}
