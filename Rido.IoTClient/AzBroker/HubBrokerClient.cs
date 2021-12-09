using MQTTnet;
using MQTTnet.Client;
using Rido.IoTClient.AzBroker.TopicBindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzBroker
{
    public class HubBrokerClient
    {
        public IMqttClient Connection;
        public string InitialTwin = string.Empty;

        public ConnectionSettings ConnectionSettings;
        readonly GetTwinBinder GetTwinBinder;
        readonly UpdateTwinBinder UpdateTwinBinder;

        public HubBrokerClient(IMqttClient c)
        {
            Connection = c;
            GetTwinBinder = new GetTwinBinder(c);
            UpdateTwinBinder = new UpdateTwinBinder(c);
        }

        public static async Task<HubBrokerClient> CreateAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            //await ProvisionIfNeededAsync(cs);
            IMqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient();
            var connAck = await mqtt.ConnectAsync(new MqttClientOptionsBuilder().WithAzureIoTHubBrokerCredentials(cs).Build(), cancellationToken);
            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }
            return new HubBrokerClient(mqtt);
        }

        public async Task<string> GetTwinAsync(CancellationToken cancellationToken = default) => await GetTwinBinder.GetTwinAsync(cancellationToken);

        public async Task<int> UpdateTwinAsync(object payload, CancellationToken cancellationToken = default) => await UpdateTwinBinder.UpdateTwinAsync(payload, cancellationToken);

    }
}
