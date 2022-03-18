using System.Text.Json.Serialization;

namespace Rido.Mqtt.HubClient.Dps
{
    public class RegistrationState
    {
        [JsonPropertyName("registrationId")]
        public string RegistrationId { get; set; }

        [JsonPropertyName("assignedHub")]
        public string AssignedHub { get; set; }

        [JsonPropertyName("deviceId")]
        public string DeviceId { get; set; }

        [JsonPropertyName("subStatus")]
        public string SubStatus { get; set; }

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
