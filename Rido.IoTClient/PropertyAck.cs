using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rido.IoTClient
{
    public class PropertyAck<T>
    {
        readonly string propName;
        readonly string compName;
        public PropertyAck(string name) : this(name, "") { }

        public PropertyAck(string name, string component)
        {
            propName = name;
            compName = component;
        }

        [JsonIgnore]
        public int? DesiredVersion { get; set; }
        [JsonPropertyName("av")]
        public int? Version { get; set; }
        [JsonPropertyName("ad")]
        public string Description { get; set; }
        [JsonPropertyName("ac")]
        public int Status { get; set; }
        [JsonPropertyName("value")]
        public T Value { get; set; } = default;

        public string ToAck()
        {
            if (string.IsNullOrEmpty(compName))
            {
                return JsonSerializer.Serialize(new Dictionary<string, object>() { { propName, this } });
            }
            else
            {
                Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>
                {
                    { compName, new Dictionary<string, object>() }
                };
                dict[compName].Add("__t", "c");
                dict[compName].Add(propName, this);
                return JsonSerializer.Serialize(dict);
            }
        }
    }
}
