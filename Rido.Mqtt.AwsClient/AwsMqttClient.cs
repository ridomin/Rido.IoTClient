using Rido.Mqtt.AwsClient.TopicBindings;
using Rido.MqttCore;
using Rido.MqttCore.PnP;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.AwsClient
{
    public class AwsMqttClient 
    {
        public IMqttBaseClient Connection { get; private set; }
        private readonly IPropertyStoreReader getShadowBinder;
        private readonly IPropertyStoreWriter updateShadowBinder;

        public AwsMqttClient(IMqttBaseClient c) //: base(c)
        {
            Connection = c;
            getShadowBinder = new GetShadowBinder(c);
            updateShadowBinder = new UpdateShadowBinder(c);
        }

        public Task<string> GetShadowAsync(CancellationToken cancellationToken = default) => getShadowBinder.ReadPropertiesDocAsync(cancellationToken);
        public Task<int> UpdateShadowAsync(object payload, CancellationToken cancellationToken = default) => updateShadowBinder.ReportPropertyAsync(payload, cancellationToken);
    }
}
