using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Implementations;
using Rido.IoTClient.AzDps;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub
{
    public class HubClient
    {
        public IMqttClient Connection;

        public ConnectionSettings ConnectionSettings;
        public string InitialTwin = string.Empty;

        protected GetTwinBinder GetTwinBinder;
        protected UpdateTwinBinder UpdateTwinBinder;

        public HubClient(IMqttClient connection)
        {
            this.Connection = connection;
            GetTwinBinder = new GetTwinBinder(connection);
            UpdateTwinBinder = new UpdateTwinBinder(connection);
        }

        public async Task<string> GetTwinAsync(CancellationToken cancellationToken = default) => await GetTwinBinder.GetTwinAsync(cancellationToken);

        public async Task<int> UpdateTwinAsync(object payload, CancellationToken cancellationToken = default) => await UpdateTwinBinder.UpdateTwinAsync(payload, cancellationToken);

        protected static async Task<IMqttClient> CreateAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            await ProvisionIfNeededAsync(cs);
            IMqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient(new MqttClientAdapterFactory());
            var connAck = await mqtt.ConnectAsync(new MqttClientOptionsBuilder().WithAzureIoTHubCredentials(cs).Build(), cancellationToken);
            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }
            return mqtt;
        }

        private static async Task ProvisionIfNeededAsync(ConnectionSettings dcs)
        {
            if (!string.IsNullOrEmpty(dcs.IdScope))
            {
                DpsStatus dpsResult;
                if (!string.IsNullOrEmpty(dcs.SharedAccessKey))
                {
                    dpsResult = await DpsClient.ProvisionWithSasAsync(dcs.IdScope, dcs.DeviceId, dcs.SharedAccessKey, dcs.ModelId);
                }
                else if (!string.IsNullOrEmpty(dcs.X509Key))
                {
                    var segments = dcs.X509Key.Split('|');
                    string pfxpath = segments[0];
                    string pfxpwd = segments[1];
                    dpsResult = await DpsClient.ProvisionWithCertAsync(dcs.IdScope, pfxpath, pfxpwd, dcs.ModelId);
                }
                else
                {
                    throw new ApplicationException("No Key found to provision");
                }

                if (!string.IsNullOrEmpty(dpsResult.registrationState.assignedHub))
                {
                    dcs.HostName = dpsResult.registrationState.assignedHub;
                }
                else
                {
                    throw new ApplicationException("DPS Provision failed: " + dpsResult.status);
                }
            }
        }



    }
}
