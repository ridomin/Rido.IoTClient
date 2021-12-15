using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient
{
    public interface IUpdatePropoertyBinder
    {
        Task<int> UpdatePropertyAsync(object payload, CancellationToken cancellationToken = default);
    }
}