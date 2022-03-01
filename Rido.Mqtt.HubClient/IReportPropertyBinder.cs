using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient
{
    public interface IReportPropertyBinder
    {
        Task<int> ReportPropertyAsync(string payload, CancellationToken cancellationToken = default);
    }
}