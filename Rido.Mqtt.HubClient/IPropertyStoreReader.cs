using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient
{
    public interface IPropertyStoreReader
    {
        Task<string> ReadPropertiesDocAsync(CancellationToken cancellationToken = default);
    }
}