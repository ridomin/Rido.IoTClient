using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient
{
    public interface IPropertyStoreReaderBinder
    {
        Task<string> ReadPropertiesDocAsync(CancellationToken cancellationToken = default);
    }
}