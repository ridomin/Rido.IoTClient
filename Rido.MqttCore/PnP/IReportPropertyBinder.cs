using System.Threading;
using System.Threading.Tasks;

namespace Rido.MqttCore.PnP
{
    /// <summary>
    /// IReportPropertyBinder
    /// </summary>
    public interface IReportPropertyBinder
    {
        /// <summary>
        /// ReportPropertyAsync
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default);
    }
}