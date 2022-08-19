using System.Text.Json.Serialization;

namespace Rido.Mqtt.AzIoTClient
{
    public class DpsStatus
    {
        [JsonPropertyName("operationId")]
        public string OperationId { get; set; } = string.Empty;
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("registrationState")]
        public RegistrationState RegistrationState { get; set; } = new RegistrationState();
    }
}
