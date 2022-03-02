using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.MqttCore
{
    public class MqttMessage
    {
        public string Topic { get; set; } = "";
        public string Payload { get; set; } = "";
    }

    public class DisconnectEventArgs
    {
        public string ReasonInfo { get; set; } = string.Empty;
    }

    public interface IMqttBaseClient
    {
        ConnectionSettings ConnectionSettings { get; }
        //static Task<IMqttBaseClient> ConnectAsync(ConnectionSettings cs, CancellationToken cancellationToken = default);
        bool IsConnected { get; }
        string ClientId { get; }
        Task<int> PublishAsync(string topic, object payload, int qos = 0, CancellationToken token = default);
        Task<int> SubscribeAsync(string topic, CancellationToken token = default);
        event EventHandler<DisconnectEventArgs> OnMqttClientDisconnected;
        event Func<MqttMessage, Task> OnMessage;
    }
}
