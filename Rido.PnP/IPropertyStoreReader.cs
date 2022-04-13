using System.Threading;
using System.Threading.Tasks;

namespace Rido.PnP
{
    public interface IPropertyStoreReader
    {
        Task<string> ReadPropertiesDocAsync(CancellationToken cancellationToken = default);
    }
}
