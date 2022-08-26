using System;
using System.Text;
using System.Text.Json.Serialization;
using static Rido.MqttCore.JsonSerializerWithEnums;

namespace Rido.MqttCore.Birth
{
    /// <summary>
    /// PnP convention to announce device connectivity status and model-id
    /// 
    /// device will publish to pnp/{clientId}/birth 
    /// connect with LWT
    /// </summary>
    public class BirthConvention
    {
        /// <summary>
        /// Connection status
        /// </summary>
        public enum ConnectionStatus 
        { 
            /// <summary>
            /// Offline
            /// </summary>
            offline, 
            /// <summary>
            /// OnLine
            /// </summary>
            online 
        }

        /// <summary>
        /// Birth Topic as pnp/{clientId}/birth
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static string BirthTopic(string clientId) => $"pnp/{clientId}/birth";

        /// <summary>
        /// Birth Message
        /// </summary>
        public class BirthMessage
        {
            /// <summary>
            /// Bith message with connection status
            /// </summary>
            /// <param name="connectionStatus"></param>
            public BirthMessage(ConnectionStatus connectionStatus)
            {
                ConnectionStatus = connectionStatus;
                When = DateTime.Now;
            }
            /// <summary>
            /// ModelId as DTMI
            /// </summary>
            [JsonPropertyName("model-id")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string ModelId { get; set; }
            /// <summary>
            /// When the devices connected
            /// </summary>
            [JsonPropertyName("when")]
            public DateTime When { get; private set; }
            /// <summary>
            /// Connection Status
            /// </summary>
            [JsonPropertyName("status")]
            public ConnectionStatus ConnectionStatus { get; private set; }
        }

        /// <summary>
        /// LastWillPayload
        /// </summary>
        /// <returns></returns>
        public static byte[] LastWillPayload() => Encoding.UTF8.GetBytes(Stringify(new BirthMessage(ConnectionStatus.offline)));
        /// <summary>
        /// LastWillPayload
        /// </summary>
        /// <param name="modelId">PnP Model Id</param>
        /// <returns></returns>
        public static byte[] LastWillPayload(string modelId) => Encoding.UTF8.GetBytes(Stringify(new BirthMessage(ConnectionStatus.offline) { ModelId = modelId}));
    }
}
