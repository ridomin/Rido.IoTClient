﻿using Rido.MqttCore;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace Rido.Mqtt.M2MAdapter
{
    public class M2MClient : IMqttConnection
    {
        private readonly MqttClient client;

        public M2MClient(MqttClient c)
        {
            client = c;

            client.MqttMsgPublishReceived += (sender, e) => OnMessage?.Invoke(new MqttMessage() { Topic = e.Topic, Payload = Encoding.UTF8.GetString(e.Message) });
            client.ConnectionClosed += (sender, e) => OnMqttClientDisconnected?.Invoke(sender, new DisconnectEventArgs() { ReasonInfo = "m2m does not provide disconnect info" });
        }

        public bool IsConnected => client.IsConnected;

        public string ClientId => client.ClientId;

        public ConnectionSettings ConnectionSettings { get; set; }

        public string BaseClientLibraryInfo => client.GetType().Assembly.FullName.ToLowerInvariant();

        public event EventHandler<DisconnectEventArgs> OnMqttClientDisconnected;
        public event Func<MqttMessage, Task> OnMessage;

        public Task DisconnectAsync(CancellationToken token = default)
        {
            client.Disconnect();
            return Task.CompletedTask;
        }

        public async Task<int> PublishAsync(string topic, object payload, int qos = 1, bool retain = false, CancellationToken token = default)
        {
            string jsonPayload;

            if (payload is string)
            {
                jsonPayload = payload as string;
            }
            else
            {
                jsonPayload = JsonSerializerWithEnums.Stringify(payload);
            }

            var res = client.Publish(topic, Encoding.UTF8.GetBytes(jsonPayload), Convert.ToByte(qos), retain);
            //return await Task.FromResult(Convert.ToInt32(res==2 ? 0 : res));
            return await Task.FromResult(0);
        }

        public async Task<int> SubscribeAsync(string topic, CancellationToken token = default)
        {
            var res = client.Subscribe(new string[] { topic }, new byte[] { 1 });
            return await Task.FromResult(Convert.ToInt32(res));
        }


        public async Task<int> UnsubscribeAsync(string topic, CancellationToken token = default)
        {
            var res = client.Unsubscribe(new string[] { topic });
            return await Task.FromResult(Convert.ToInt32(res));
        }
    }
}
