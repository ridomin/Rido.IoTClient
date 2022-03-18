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
        bool IsConnected { get; }
        string ClientId { get; }
        string BaseClientLibraryInfo { get; }
        ConnectionSettings ConnectionSettings { get; }
        Task<int> PublishAsync(string topic, object payload, int qos = 1, CancellationToken token = default);
        Task<int> SubscribeAsync(string topic, CancellationToken token = default);
        Task<int> UnsubscribeAsync(string topic, CancellationToken token = default);
        Task DisconnectAsync(CancellationToken token = default);
        event Func<MqttMessage, Task> OnMessage;
        event EventHandler<DisconnectEventArgs> OnMqttClientDisconnected;
    }
}
