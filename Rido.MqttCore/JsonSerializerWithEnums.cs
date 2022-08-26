
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rido.MqttCore
{
    /// <summary>
    /// Configures Json serializer enum behavior
    /// </summary>
    public class JsonSerializerWithEnums
    {
        /// <summary>
        /// Serialize With Enums
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string Stringify(object o ) => JsonSerializer.Serialize(o,
            new JsonSerializerOptions()
            {
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            });
    }
}
