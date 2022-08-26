using MQTTnet;
using MQTTnet.Client;
using Rido.MqttCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.MqttNet4Adapter
{
    public class MqttNetClient : IMqttConnection
    {
        public bool IsConnected => connection.IsConnected;

        public string ClientId => connection.Options.ClientId;

        public ConnectionSettings ConnectionSettings { get; set; }

        public string BaseClientLibraryInfo => connection.GetType().Assembly.FullName.ToLowerInvariant();

        public event EventHandler<DisconnectEventArgs> OnMqttClientDisconnected;
        public event Func<MqttMessage, Task> OnMessage;

        private readonly IMqttClient connection;

        internal readonly List<string> subscriptions = new List<string>();
        public MqttNetClient(IMqttClient _conn)
        {
            this.connection = _conn;
            this.connection.ApplicationMessageReceivedAsync += async m =>
            {
                await OnMessage?.Invoke(
                    new MqttMessage()
                    {
                        Topic = m.ApplicationMessage.Topic,
                        Payload = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>())
                    });
            };

            this.connection.DisconnectedAsync += async d =>
            {
                OnMqttClientDisconnected?.Invoke(_conn, new DisconnectEventArgs() { ReasonInfo = d.Reason.ToString() });
                await Task.Yield();
            };
        }



        public async Task<int> PublishAsync(string topic, object payload, int qos = 0, bool retain = false, CancellationToken token = default)
        {
            if (!connection.IsConnected)
            {
                Trace.TraceWarning("Missing one message to " + topic);
                return 1;
            }

            string jsonPayload;

            if (payload is string)
            {
                jsonPayload = payload as string;
            }
            else
            {
                jsonPayload = JsonSerializerWithEnums.Stringify(payload);
            }

            var res = await connection.PublishAsync(
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
            if (!subscriptions.Contains(topic))
            {
                subscriptions.Add(topic);
                var res = await connection.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter(topic).Build(), token);
                var errs = res.Items.ToList().Any(x => x.ResultCode > MqttClientSubscribeResultCode.GrantedQoS2);
                if (errs)
                {
                    throw new ApplicationException("Error subscribing to " + topic);
                }
            }
            return 0;
        }

        public async Task<int> UnsubscribeAsync(string topic, CancellationToken token = default)
        {
            var res = await connection.UnsubscribeAsync(new MqttClientUnsubscribeOptionsBuilder().WithTopicFilter(topic).Build(), token);
            var errs = res.Items.ToList().Any(x => x.ResultCode > MqttClientUnsubscribeResultCode.Success);
            if (errs)
            {
                throw new ApplicationException("Error unsubscribing to " + topic);
            }
            //if (subscriptions.Contains(topic))
            //{
            //    subscriptions.Remove(topic);
            //}
            return 0;
        }

        public async Task DisconnectAsync(CancellationToken token = default)
        {
            foreach (var topic in subscriptions)
            {
                await UnsubscribeAsync(topic, token);
            }
            await connection.DisconnectAsync(new MqttClientDisconnectOptionsBuilder()
                //.WithReason(MqttClientDisconnectReason.UnspecifiedError)
                .Build(), token);
        }
    }
}
