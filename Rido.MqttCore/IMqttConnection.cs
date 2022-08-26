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
    
    /// <summary>
    /// MQTT Abstraction to support different MQTT Clients
    /// </summary>
    public interface IMqttConnection
    {
        bool IsConnected { get; }
        string ClientId { get; }
        ConnectionSettings ConnectionSettings { get; }
        /// <summary>
        /// Publish MQTT message to a topic
        /// </summary>
        /// <param name="topic">Target topic</param>
        /// <param name="payload">Payload string (will be UTF* encoded)</param>
        /// <param name="qos">QoS, defaults to 1</param>
        /// <param name="retain">Retain flag, defaults to false</param>
        /// <param name="token">Async cancelation token</param>
        /// <returns></returns>
        Task<int> PublishAsync(string topic, object payload, int qos = 1, bool retain = false, CancellationToken token = default);
        /// <summary>
        /// Subscribes to a MQTT topic
        /// </summary>
        /// <param name="topic">Topic to subscribe</param>
        /// <param name="token">Async cancelation token</param>
        /// <returns></returns>
        Task<int> SubscribeAsync(string topic, CancellationToken token = default);
        Task<int> UnsubscribeAsync(string topic, CancellationToken token = default);
        Task DisconnectAsync(CancellationToken token = default);
        /// <summary>
        /// Callback to receive messages from subscribed topics
        /// </summary>
        event Func<MqttMessage, Task> OnMessage;
        event EventHandler<DisconnectEventArgs> OnMqttClientDisconnected;
    }
}
