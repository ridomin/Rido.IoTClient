using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.MqttCore
{
    /// <summary>
    /// Mqtt Message
    /// </summary>
    public class MqttMessage
    {
        /// <summary>
        /// Topic
        /// </summary>
        public string Topic { get; set; } = "";
        /// <summary>
        /// Payload from UTF8
        /// </summary>
        public string Payload { get; set; } = "";
    }

    /// <summary>
    /// Disconnection Event
    /// </summary>
    public class DisconnectEventArgs
    {
        // TODO: Refactor disconnect events
        /// <summary>
        /// Reason
        /// </summary>
        public string ReasonInfo { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// MQTT Abstraction to support different MQTT Clients
    /// </summary>
    public interface IMqttConnection
    {
        /// <summary>
        /// Returns if the connection is active
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// Client Id
        /// </summary>
        string ClientId { get; }
        /// <summary>
        /// Current Connection Settings
        /// </summary>
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
        /// <summary>
        /// Unsuscribe
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<int> UnsubscribeAsync(string topic, CancellationToken token = default);
        /// <summary>
        /// Disconnect
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task DisconnectAsync(CancellationToken token = default);
        /// <summary>
        /// Callback to receive messages from subscribed topics
        /// </summary>
        event Func<MqttMessage, Task> OnMessage;
        /// <summary>
        /// Callback for client disconnects
        /// </summary>
        event EventHandler<DisconnectEventArgs> OnMqttClientDisconnected;
    }
}
