using Rido.MqttCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace Rido.Mqtt.M2MAdapter
{
    public static class M2MClientConnectionFactory
    {
        public static async Task<IMqttBaseClient> CreateHubClientAsync(string connectionSettingsString, CancellationToken cancellationToken = default)
        {
            var connectionSettings = new ConnectionSettings(connectionSettingsString); ;
            var mqtt = new MqttClient(connectionSettings.HostName, 8883, true, MqttSslProtocols.TLSv1_2, null, null);
            (string u, string p) = SasAuth.GenerateHubSasCredentials(connectionSettings.HostName, connectionSettings.DeviceId, connectionSettings.SharedAccessKey, connectionSettings.ModelId, connectionSettings.SasMinutes);
            int res = mqtt.Connect(connectionSettings.DeviceId, u, p);
            Console.WriteLine(res);
            return await Task.FromResult(new M2MClient(mqtt) { ConnectionSettings = connectionSettings });
        }

        public static async Task<IMqttBaseClient> CreateBrokerClientAsync(string connectionSettingsString, CancellationToken cancellationToken = default)
        {
            var connectionSettings = new ConnectionSettings(connectionSettingsString); ;
            var mqtt = new MqttClient(connectionSettings.HostName, 8883, true, MqttSslProtocols.TLSv1_2, null, null);
            int res = mqtt.Connect(connectionSettings.DeviceId, connectionSettings.DeviceId, connectionSettings.SharedAccessKey);
            Console.WriteLine(res);
            return await Task.FromResult(new M2MClient(mqtt) { ConnectionSettings = connectionSettings });
        }
    }
}
