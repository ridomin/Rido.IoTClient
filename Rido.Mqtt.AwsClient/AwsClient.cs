using Rido.Mqtt.AwsClient.TopicBindings;
using Rido.MqttCore;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.AwsClient
{
    public class AwsClient //: BaseClient
    {

        readonly IPropertyStoreReader getShadowBinder;
        readonly IPropertyStoreWriter updateShadowBinder;

        public AwsClient(IMqttBaseClient c) //: base(c)
        {
            getShadowBinder = new GetShadowBinder(c);
            updateShadowBinder = new UpdateShadowBinder(c);
        }

        public Task<string> GetShadowAsync(CancellationToken cancellationToken = default) => getShadowBinder.ReadPropertiesDocAsync(cancellationToken);
        public Task<int> UpdateShadowAsync(object payload, CancellationToken cancellationToken = default) => updateShadowBinder.ReportPropertyAsync(payload, cancellationToken);
    }
}
