
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rido.MqttCore
{
    public class JsonSerializerWithEnums
    {
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
