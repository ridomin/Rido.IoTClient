using System.Text.Json.Serialization;

namespace Rido.Mqtt.Client
{
    public abstract class BaseCommandResponse
    {
        [JsonIgnore]
        public int Status { get; set; }
    }
}
