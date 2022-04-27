using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.MqttCore.PnP
{
    public interface IComponent
    {
        Task<int> ReportPropertyAsync(CancellationToken token = default);
        Dictionary<string, object> ToJsonDict();
    }
}