using Rido.MqttCore;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace Rido.Mqtt.M2MAdapter
{
    public class M2MClient : IMqttBaseClient
    {
        private readonly MqttClient client;
        private static ConnectionSettings connectionSettings;

        public M2MClient(MqttClient c)
        {
            client = c;

            client.MqttMsgPublishReceived += (sender, e) => OnMessage?.Invoke(new MqttMessage() { Topic = e.Topic, Payload = Encoding.UTF8.GetString(e.Message) });
            client.ConnectionClosed += (sender, e) => OnMqttClientDisconnected?.Invoke(sender, new DisconnectEventArgs() { ReasonInfo = "m2m does not provide disconnect info" });
        }

        public bool IsConnected => client.IsConnected;

        public string ClientId => client.ClientId;

        public ConnectionSettings ConnectionSettings { get => connectionSettings; }

        public event EventHandler<DisconnectEventArgs> OnMqttClientDisconnected;
        public event Func<MqttMessage, Task> OnMessage;

        public async Task<int> PublishAsync(string topic, object payload, int qos = 0, CancellationToken token = default)
        {
            string jsonPayload;

            if (payload is string)
            {
                jsonPayload = payload as string;
            }
            else
            {
                jsonPayload = JsonSerializer.Serialize(payload);
            }

            var res = client.Publish(topic, Encoding.UTF8.GetBytes(jsonPayload));
            //return await Task.FromResult(Convert.ToInt32(res==2 ? 0 : res));
            return await Task.FromResult(0);
        }

        public async Task<int> SubscribeAsync(string topic, CancellationToken token = default)
        {
            var res = client.Subscribe(new string[] { topic }, new byte[] { 0 });
            return await Task.FromResult(Convert.ToInt32(res));
        }

        public static async Task<IMqttBaseClient> CreateAsync(string connectionSettingsString, CancellationToken cancellationToken = default)
        {
            connectionSettings = new ConnectionSettings(connectionSettingsString); ;
            var mqtt = new MqttClient(connectionSettings.HostName, 8883, true, MqttSslProtocols.TLSv1_2, null, null);
            (string u, string p) = SasAuth.GenerateHubSasCredentials(connectionSettings.HostName, connectionSettings.DeviceId, connectionSettings.SharedAccessKey, connectionSettings.ModelId, connectionSettings.SasMinutes);
            int res = mqtt.Connect(connectionSettings.DeviceId, u, p);
            Console.WriteLine(res);
            return await Task.FromResult(new M2MClient(mqtt));
        }
    }
}
