using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient
{
    public interface IReadOnlyProperty<T>
    {
        T PropertyValue { get; set; }
        Task<int> ReportPropertyAsync(CancellationToken cancellationToken = default);
    }
}