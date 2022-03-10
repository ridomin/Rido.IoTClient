using System.Threading;
using System.Threading.Tasks;

namespace Rido.MqttCore
{
    public interface IHubClientConnectionFactory
    {
        Task<IMqttBaseClient> CreateHubClientAsync(string connectionSettingsString, CancellationToken cancellationToken = default);
    }
}