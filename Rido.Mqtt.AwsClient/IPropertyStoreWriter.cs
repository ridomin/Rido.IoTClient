using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.AwsClient
{
    public interface IPropertyStoreWriter
    {
        Task<int> ReportPropertyAsync(object payload, CancellationToken token = default);
    }
}
