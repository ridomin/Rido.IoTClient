using System.Text.Json.Serialization;

namespace Rido.MqttCore.PnP
{
    public abstract class BaseCommandResponse
    {
        [JsonIgnore]
        public int Status { get; set; }
    }
}
