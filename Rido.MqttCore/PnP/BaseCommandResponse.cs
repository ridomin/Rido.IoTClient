using System.Text.Json.Serialization;

namespace Rido.MqttCore.PnP
{
    /// <summary>
    /// Base class for custom PnP Command Responses
    /// </summary>
    public abstract class BaseCommandResponse
    {
        /// <summary>
        /// Command Response Status
        /// </summary>
        [JsonIgnore]
        public int Status { get; set; }
    }
}
