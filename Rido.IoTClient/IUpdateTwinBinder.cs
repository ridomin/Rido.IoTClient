using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient
{
    public interface IUpdateTwinBinder
    {
        Task<int> UpdateTwinAsync(object payload, CancellationToken cancellationToken = default);
    }
}