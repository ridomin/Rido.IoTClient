using Rido.Mqtt.DpsClient;
using Rido.Mqtt.HubClient;
using Rido.MqttCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.AzIoTClient
{
    public class HubDpsFactory
    {
        public static async Task<HubMqttClient> CreateFromConnectionStringAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            var cs = new ConnectionSettings(connectionString);
            if (string.IsNullOrEmpty(cs.HostName) && !string.IsNullOrEmpty(cs.IdScope))
            {
                var dpsMqtt = await new MqttNet4Adapter.MqttNetClientConnectionFactory().CreateDpsClientAsync(connectionString, cancellationToken);
                var dpsClient = new MqttDpsClient(dpsMqtt);
                var dpsRes = await dpsClient.ProvisionDeviceIdentity();
                cs.HostName = dpsRes.RegistrationState.AssignedHub;
                cs.ClientId = dpsRes.RegistrationState.DeviceId;
                await dpsMqtt.DisconnectAsync(cancellationToken);
            }
            var mqtt = await new MqttNet4Adapter.MqttNetClientConnectionFactory().CreateHubClientAsync(cs);
            return new HubMqttClient(mqtt);
        }
    }
}
