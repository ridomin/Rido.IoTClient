using System.Text.Json.Serialization;

namespace Rido.Mqtt.HubClient
{
    public abstract class BaseCommandResponse
    {
        [JsonIgnore]
        public int Status { get; set; }
    }
}
