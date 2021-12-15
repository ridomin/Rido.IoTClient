using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient
{
    public interface IReportPropoertyBinder
    {
        Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default);
    }
}