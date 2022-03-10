using MQTTnet;
using MQTTnet.Client;
using Rido.MqttCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.MqttNetAdapter
{
    public class MqttNetClient : IMqttBaseClient
    {
        //private static ConnectionSettings connectionSettings;
        public bool IsConnected => client.IsConnected;

        public string ClientId => client.Options.ClientId;

        public ConnectionSettings ConnectionSettings { get; set; }

        public event EventHandler<DisconnectEventArgs> OnMqttClientDisconnected;
        public event Func<MqttMessage, Task> OnMessage;

        private readonly MqttClient client;
        public MqttNetClient(MqttClient client)
        {
            this.client = client;
            this.client.ApplicationMessageReceivedAsync += async m =>
            {
                await OnMessage.Invoke(
                    new MqttMessage()
                    {
                        Topic = m.ApplicationMessage.Topic,
                        Payload = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>())
                    });
            };

            this.client.DisconnectedAsync += async d =>
            {
                OnMqttClientDisconnected.Invoke(client, new DisconnectEventArgs() { ReasonInfo = d.Reason.ToString() });
                await Task.Yield();
            };
        }

       

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

            var res = await client.PublishAsync(
                new MqttApplicationMessage()
                {
                    Topic = topic,
                    Payload = Encoding.UTF8.GetBytes(jsonPayload)
                },
                token);

            if (res.ReasonCode != MqttClientPublishReasonCode.Success)
            {
                throw new ApplicationException("Error publishing to " + topic);
            }
            return 0;
        }

        public async Task<int> SubscribeAsync(string topic, CancellationToken token = default)
        {
            var res = await client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter(topic).Build(), token);
            var errs = res.Items.ToList().Any(x => x.ResultCode > MqttClientSubscribeResultCode.GrantedQoS2);
            if (errs)
            {
                throw new ApplicationException("Error subscribing to " + topic);
            }
            return 0;
        }
    }
}
