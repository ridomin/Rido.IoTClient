using System.Threading;
using System.Threading.Tasks;

namespace Rido.MqttCore.PnP
{
    public interface IReadOnlyProperty<T>
    {
        string PropertyName { get; }
        T PropertyValue { get; set; }
        int Version { get; set; }
        Task<int> ReportPropertyAsync(CancellationToken cancellationToken = default);
    }
}