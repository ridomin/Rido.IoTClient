using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
namespace Rido.MqttCore.PnP
{
    /// <summary>
    /// Implements PnP Properties convention
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyAck<T>
    {
        /// <summary>
        /// Property Name
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Component Name (optional)
        /// </summary>
        public readonly string ComponentName;

        /// <summary>
        /// Ctor requires property name
        /// </summary>
        /// <param name="name">property name</param>
        public PropertyAck(string name) : this(name, "") { }

        /// <summary>
        /// Ctor for component
        /// </summary>
        /// <param name="name">property name</param>
        /// <param name="component">component name</param>
        public PropertyAck(string name, string component)
        {
            Name = name;
            ComponentName = component;
        }

        /// <summary>
        /// Desired Version 
        /// </summary>
        [JsonIgnore]
        public int? DesiredVersion { get; set; }

        /// <summary>
        /// Last reported property
        /// </summary>
        [JsonIgnore]
        public T LastReported { get; set; }

        /// <summary>
        /// PnP ACK convention Version (av)
        /// </summary>
        [JsonPropertyName("av")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Version { get; set; }

        /// <summary>
        /// PnP ACK convention Description (ad)
        /// </summary>
        [JsonPropertyName("ad")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Description { get; set; }

        /// <summary>
        /// PnP ACK convention Status (ac)
        /// </summary>
        [JsonPropertyName("ac")]
        public int Status { get; set; }

        /// <summary>
        /// PnP ACK convention Value (value)
        /// </summary>
        [JsonPropertyName("value")]
        public T Value { get; set; } = default;

        /// <summary>
        /// Set Default value
        /// </summary>
        /// <param name="defaultValue"></param>
        public void SetDefault(T defaultValue)
        {
            Value = defaultValue;
            Version = 0;
            Status = 203;
            Description = "default value";
        }

        /// <summary>
        /// Convert to Dictionary to support generic serialization
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> ToAckDict()
        {
            if (string.IsNullOrEmpty(ComponentName))
            {
                return new Dictionary<string, object>() { { Name, this } };
            }
            else
            {
                Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>
                {
                    { ComponentName, new Dictionary<string, object>() }
                };
                dict[ComponentName].Add("__t", "c");
                dict[ComponentName].Add(Name, this);
                return dict.ToDictionary(pair => pair.Key, pair => (object)pair.Value);
            }
        }
    }
}
