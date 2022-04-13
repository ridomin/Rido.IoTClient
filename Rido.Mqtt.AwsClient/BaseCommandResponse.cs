using System.Text.Json.Serialization;

namespace Rido.Mqtt.AwsClient
{
    public abstract class BaseCommandResponse
    {
        [JsonIgnore]
        public int Status { get; set; }
    }
}
