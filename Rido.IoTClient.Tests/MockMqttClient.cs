using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Tests
{
    class MockMqttClient : IMqttClient
    {
        public MockMqttClient()
        {
         
        }

        public bool IsConnected => throw new NotImplementedException();

        public IMqttClientOptions Options
        {
            get => new MqttClientOptions() { ClientId = "mock"};
        }

        public IMqttClientConnectedHandler ConnectedHandler { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IMqttClientDisconnectedHandler DisconnectedHandler { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event Func<MqttClientConnectedEventArgs, Task> ConnectedAsync;
        public event Func<MqttClientDisconnectedEventArgs, Task> DisconnectedAsync;
        public event Func<MqttApplicationMessageReceivedEventArgs, Task> ApplicationMessageReceivedAsync;

        public Task<MqttClientConnectResult> ConnectAsync(IMqttClientOptions options, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DisconnectAsync(MqttClientDisconnectOptions options, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task PingAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public string payloadReceived;
        public string topicRecceived;
        public Task<MqttClientPublishResult> PublishAsync(MqttApplicationMessage applicationMessage, CancellationToken cancellationToken = default)
        {
            topicRecceived = applicationMessage.Topic;
            payloadReceived = Encoding.UTF8.GetString(applicationMessage.Payload);
            return Task.FromResult(new MqttClientPublishResult() { ReasonCode = MqttClientPublishReasonCode.Success });
        }

        public Task SendExtendedAuthenticationExchangeDataAsync(MqttExtendedAuthenticationExchangeData data, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<MqttClientSubscribeResult> SubscribeAsync(MqttClientSubscribeOptions options, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new MqttClientSubscribeResult());
            //throw new NotImplementedException();
        }

        public Task<MqttClientUnsubscribeResult> UnsubscribeAsync(MqttClientUnsubscribeOptions options, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
