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
        public string InitialTwin = string.Empty;

        public ConnectionSettings ConnectionSettings;
        readonly GetTwinBinder GetTwinBinder;
        readonly UpdateTwinBinder UpdateTwinBinder;

        public IoTHubBrokerPnPClient(IMqttClient c)
        {
            Connection = c;
            GetTwinBinder = new GetTwinBinder(c);
            UpdateTwinBinder = UpdateTwinBinder.GetInstance(c);
        }

        public Task<string> GetTwinAsync(CancellationToken cancellationToken = default) =>
            GetTwinBinder.GetTwinAsync(cancellationToken);

        public Task<int> UpdateTwinAsync(object payload, CancellationToken cancellationToken = default) =>
            UpdateTwinBinder.ReportPropertyAsync(payload, cancellationToken);

    }
}
