using System.Threading;
using System.Threading.Tasks;

namespace Rido.MqttCore.PnP
{
    public interface IPropertyStoreReader
    {
        Task<string> ReadPropertiesDocAsync(CancellationToken cancellationToken = default);
    }
}
