using MQTTnet;
using MQTTnet.Client;
using Rido.MqttCore;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.MqttNet4Adapter
{
    public class MqttNetClient : IMqttBaseClient
    {
        public bool IsConnected => client.IsConnected;

        public string ClientId => client.Options.ClientId;

        public ConnectionSettings ConnectionSettings { get; set; }

        public string BaseClientLibraryInfo => client.GetType().Assembly.FullName.ToLowerInvariant();

        public event EventHandler<DisconnectEventArgs> OnMqttClientDisconnected;
        public event Func<MqttMessage, Task> OnMessage;

        private readonly MqttClient client;
        public MqttNetClient(MqttClient client)
        {
            this.client = client;
            this.client.ApplicationMessageReceivedAsync += async m =>
            {
                await OnMessage?.Invoke(
                    new MqttMessage()
                    {
                        Topic = m.ApplicationMessage.Topic,
                        Payload = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>())
                    });
            };

            this.client.DisconnectedAsync += async d =>
            {
                OnMqttClientDisconnected?.Invoke(client, new DisconnectEventArgs() { ReasonInfo = d.Reason.ToString() });
                await Task.Yield();
            };
        }



        public async Task<int> PublishAsync(string topic, object payload, int qos = 0, bool retain = false, CancellationToken token = default)
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
                new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(jsonPayload)
                    .WithRetainFlag(retain)
                    .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
                    .Build(),
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

        public async Task<int> UnsubscribeAsync(string topic, CancellationToken token = default)
        {
            var res = await client.UnsubscribeAsync(new MqttClientUnsubscribeOptionsBuilder().WithTopicFilter(topic).Build(), token);
            var errs = res.Items.ToList().Any(x => x.ResultCode > MqttClientUnsubscribeResultCode.Success);
            if (errs)
            {
                throw new ApplicationException("Error unsubscribing to " + topic);
            }
            return 0;
        }

        public async Task DisconnectAsync(CancellationToken token = default)
        {
            await client.DisconnectAsync(new MqttClientDisconnectOptionsBuilder()
                //.WithReason(MqttClientDisconnectReason.UnspecifiedError)
                .Build(), token);
        }
    }
}
