using System.Text.Json.Serialization;

namespace Rido.PnP
{
    public abstract class BaseCommandResponse
    {
        [JsonIgnore]
        public int Status { get; set; }
    }
}
