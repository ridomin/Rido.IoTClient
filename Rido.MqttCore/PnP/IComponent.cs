using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.MqttCore.PnP
{
    /// <summary>
    /// Interface for PnP Components
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// Submit a property related to a component
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<int> ReportPropertyAsync(CancellationToken token = default);
        /// <summary>
        /// Convert to Dictionary to support JSON serialization
        /// </summary>
        /// <returns></returns>
        Dictionary<string, object> ToJsonDict();
    }
}