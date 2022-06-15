using Rido.Mqtt.DpsClient;
using Rido.Mqtt.HubClient;
using Rido.MqttCore;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.AzIoTClient
{
    public class HubDpsFactory
    {
        static IMqttBaseClient connection;
        static HubMqttClient hubClient;
        static ConnectionSettings connectionSettings;
        static Timer reconnectTimer;
        public static EventHandler OnReconnect;
        public static async Task<HubMqttClient> CreateFromConnectionStringAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            connectionSettings = new ConnectionSettings(connectionString);
            if (string.IsNullOrEmpty(connectionSettings.HostName) && !string.IsNullOrEmpty(connectionSettings.IdScope))
            {
                var dpsMqtt = await new MqttNet4Adapter.MqttNetClientConnectionFactory().CreateDpsClientAsync(connectionString, cancellationToken);
                var dpsClient = new MqttDpsClient(dpsMqtt);
                var dpsRes = await dpsClient.ProvisionDeviceIdentity();
                connectionSettings.HostName = dpsRes.RegistrationState.AssignedHub;
                connectionSettings.ClientId = dpsRes.RegistrationState.DeviceId;
                await dpsMqtt.DisconnectAsync(cancellationToken);
            }
            if (connectionSettings.Auth == AuthType.Sas)
            {
                connection = ConnectWithTimer();
            }
            else
            {
                connection = await new MqttNet4Adapter.MqttNetClientConnectionFactory().CreateHubClientAsync(connectionSettings);
            }    
            hubClient = new HubMqttClient(connection);
            return hubClient;
        }

       static IMqttBaseClient ConnectWithTimer()
        {
            Delegate[] currentDelegates = new Delegate[0];
            if (connection != null && connection.IsConnected)
            {
                Trace.WriteLine("Reconnecting with Sas Token Expire");
                currentDelegates = connection.GetInvocationList();
                connection.DisconnectAsync().Wait();
            }
            connection = new MqttNet4Adapter.MqttNetClientConnectionFactory().CreateHubClientAsync(connectionSettings).Result;
            foreach (var item in currentDelegates)
            {
                connection.OnMessage += (Func<MqttMessage, Task>)item;
            }
            //if (connection.IsConnected)
            //{
            //    OnReconnect?.Invoke(connection, new EventArgs());
            //}
            reconnectTimer = new Timer(o =>
            {
                ConnectWithTimer();
            }, null, 10000, 0);
            //(connectionSettings.SasMinutes * 60 * 1000) - 10, 0);
            return connection;
        }
    }
}
