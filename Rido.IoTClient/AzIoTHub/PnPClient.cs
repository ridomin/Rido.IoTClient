using MQTTnet;
using MQTTnet.Client;
using Rido.IoTClient.AzDps;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub
{
    public class PnPClient //: IDisposable
    {
        public readonly IMqttClient Connection;

        public ConnectionSettings ConnectionSettings;
        public string InitialTwin = string.Empty;

        readonly GetTwinBinder GetTwinBinder;
        public readonly IUpdatePropoertyBinder updateTwinBinder;

        public PnPClient(IMqttClient connection)
        {
            this.Connection = connection;
            GetTwinBinder = new GetTwinBinder(connection);
            updateTwinBinder = UpdateTwinBinder.GetInstance(connection);
        }

        public Task<string> GetTwinAsync(CancellationToken cancellationToken = default) => GetTwinBinder.GetTwinAsync(cancellationToken);

        public Task<int> UpdateTwinAsync(object payload, CancellationToken cancellationToken = default) => updateTwinBinder.UpdatePropertyAsync(payload, cancellationToken);

        public static async Task<PnPClient> CreateAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            await DpsClient.ProvisionIfNeededAsync(cs);
            IMqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient();
            var connAck = await mqtt.ConnectAsync(new MqttClientOptionsBuilder().WithAzureIoTHubCredentials(cs).Build(), cancellationToken);
            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }
            return new PnPClient(mqtt) { ConnectionSettings = cs };
        }

        //public void Dispose()
        //{
        //    Connection.Dispose();
        //}
    }
}
