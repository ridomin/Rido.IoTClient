using MQTTnet;
using MQTTnet.Client;
using Rido.IoTClient.Aws.TopicBindings;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Aws
{
    public class AwsPnPClient
    {
        public IMqttClient Connection;
        public string InitialState = string.Empty;
        public ConnectionSettings ConnectionSettings;

        readonly IPropertyStoreReader getShadowBinder;
        readonly IPropertyStoreWriter updateShadowBinder;
        public readonly DesiredUpdatePropertyBinder<string> desiredUpdatePropertyBinder;

        public AwsPnPClient(IMqttClient c)
        {
            this.Connection = c;
            getShadowBinder = new GetShadowBinder(c);
            updateShadowBinder = UpdateShadowBinder.GetInstance(c);
            desiredUpdatePropertyBinder = new DesiredUpdatePropertyBinder<string>(c, c.Options.ClientId, "name");
        }

        public Task<string> GetShadowAsync(CancellationToken cancellationToken = default) => getShadowBinder.ReadPropertiesDocAsync(cancellationToken);
        public Task<int> UpdateShadowAsync(object payload, CancellationToken cancellationToken = default) => updateShadowBinder.ReportPropertyAsync(payload, cancellationToken);
    }
}
