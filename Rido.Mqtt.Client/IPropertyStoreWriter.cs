using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.Client
{
    public interface IPropertyStoreWriter
    {
        Task<int> ReportPropertyAsync(object payload, CancellationToken token = default);
    }
}
