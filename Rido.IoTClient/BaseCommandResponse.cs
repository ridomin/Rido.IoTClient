using System.Text.Json.Serialization;

namespace Rido.IoTClient
{
    public abstract class BaseCommandResponse
    {
        [JsonIgnore]
        public int Status { get; set; }
    }
}
