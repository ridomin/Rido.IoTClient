using System;
using System.Text.Json.Serialization;
using static Rido.MqttCore.JsonSerializerWithEnums;

namespace Rido.MqttCore.Birth
{
    public class BirthConvention
    {
        public enum ConnectionStatus { offline, online }

        public static string BirthTopic(string clientId) => $"pnp/{clientId}/birth";

        public class BirthMessage
        {
            public BirthMessage(ConnectionStatus connectionStatus)
            {
                ConnectionStatus = connectionStatus;
                When = DateTime.Now;
            }
            [JsonPropertyName("model-id")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string ModelId { get; set; }
            [JsonPropertyName("when")]
            public DateTime When { get; private set; }
            [JsonPropertyName("status")]
            public ConnectionStatus ConnectionStatus { get; private set; }
        }

        public static string LastWillPayload() => Stringify(new BirthMessage(ConnectionStatus.offline));
    }
}
