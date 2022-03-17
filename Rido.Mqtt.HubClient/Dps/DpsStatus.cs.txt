using System.Text.Json.Serialization;

namespace Rido.Mqtt.HubClient.Dps
{
    public class DpsStatus
    {
        [JsonPropertyName("operationId")]
        public string OperationId { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("registrationState")]
        public RegistrationState RegistrationState { get; set; }
    }
}
