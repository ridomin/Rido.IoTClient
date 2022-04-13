using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.Client
{
    public interface IReportPropertyBinder
    {
        Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default);
    }
}