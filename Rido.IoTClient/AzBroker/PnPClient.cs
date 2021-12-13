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
    public class PnPClient
    {
        public IMqttClient Connection;
        public string InitialTwin = string.Empty;

        public ConnectionSettings ConnectionSettings;
        readonly GetTwinBinder GetTwinBinder;
        readonly UpdateTwinBinder UpdateTwinBinder;

        public PnPClient(IMqttClient c)
        {
            Connection = c;
            GetTwinBinder = new GetTwinBinder(c);
            UpdateTwinBinder = new UpdateTwinBinder(c);
        }

        public static async Task<PnPClient> CreateAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            await DpsClient.ProvisionIfNeededAsync(cs);
            IMqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient();
            var connAck = await mqtt.ConnectAsync(new MqttClientOptionsBuilder().WithAzureIoTHubBrokerCredentials(cs).Build(), cancellationToken);
            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }
            return new PnPClient(mqtt);
        }

        public Task<string> GetTwinAsync(CancellationToken cancellationToken = default) =>
            GetTwinBinder.GetTwinAsync(cancellationToken);

        public Task<int> UpdateTwinAsync(object payload, CancellationToken cancellationToken = default) =>
            UpdateTwinBinder.UpdateTwinAsync(payload, cancellationToken);

    }
}
