using MQTTnet;
using MQTTnet.Client;
using Rido.IoTClient.AzBroker.TopicBindings;
using Rido.IoTClient.AzDps;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzBroker
{
    public class IoTHubBrokerPnPClient
    {
        public IMqttClient Connection;
        public string InitialState = string.Empty;

        public ConnectionSettings ConnectionSettings;
        readonly IPropertyStoreReader getTwinBinder;
        readonly IPropertyStoreWriter updateTwinBinder;

        public IoTHubBrokerPnPClient(IMqttClient c)
        {
            Connection = c; 
            getTwinBinder = new GetTwinBinder(c);
            updateTwinBinder = UpdateTwinBinder.GetInstance(c);
        }

        public Task<string> GetTwinAsync(CancellationToken cancellationToken = default) =>
            getTwinBinder.ReadPropertiesDocAsync(cancellationToken);

        public Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default) =>
            updateTwinBinder.ReportPropertyAsync(payload, cancellationToken);

    }
}
