using System.Text.Json.Serialization;

namespace Rido.IoTClient.AzDps
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
    }
}
