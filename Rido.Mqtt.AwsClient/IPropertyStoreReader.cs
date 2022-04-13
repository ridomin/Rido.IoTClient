using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.AwsClient
{
    public interface IPropertyStoreReader
    {
        Task<string> ReadPropertiesDocAsync(CancellationToken cancellationToken = default);
    }
}