
using Rido.MqttCore;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient.Tests
{
    internal class MockMqttClient : IMqttBaseClient
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MockMqttClient()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }

        public bool IsConnected => throw new NotImplementedException();

        public ConnectionSettings ConnectionSettings => throw new NotImplementedException();

        public string ClientId => "mock";



        public string payloadReceived;
        public string topicRecceived;

        internal int numSubscriptions = 0;

        public event EventHandler<DisconnectEventArgs> OnMqttClientDisconnected;
        public event Func<MqttMessage, Task> OnMessage;

        public void SimulateNewMessage(string topic, string payload)
        {
            var msg = new MqttMessage() { Topic = topic, Payload = payload };
            OnMessage.Invoke(msg);
        }

        public Delegate[] GetInvocationList() => OnMessage.GetInvocationList();

        public Task<int> PublishAsync(string topic, object payload, int qos = 0, CancellationToken token = default)
        {
            string jsonPayload = string.Empty;
            if (payload is string)
            {
                jsonPayload = payload as string;
            }
            else
            {
                jsonPayload = JsonSerializer.Serialize(payload);
            }

            topicRecceived = topic;
            payloadReceived = jsonPayload;// != null ? Encoding.UTF8.GetString(payload) : string.Empty;
            return Task.FromResult(0);
        }

        public Task<int> SubscribeAsync(string topic, CancellationToken token = default)
        {
            numSubscriptions++;
            //options.TopicFilters.ForEach(t => Trace.TraceInformation(t.Topic));
            return Task.FromResult(0);
        }
    }
}
