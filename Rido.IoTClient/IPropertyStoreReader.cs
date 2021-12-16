using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient
{
    public interface IPropertyStoreReader
    {
        Task<string> ReadPropertiesDocAsync(CancellationToken cancellationToken = default);
    }
}