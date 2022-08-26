using System.Text.Json.Nodes;

namespace Rido.MqttCore.PnP
{
    /// <summary>
    /// Property Parser
    /// </summary>
    public class PropertyParser
    {
        /// <summary>
        /// Finds a property from a twin document, by inspecting the name and optional component name
        /// </summary>
        /// <param name="desired"></param>
        /// <param name="propertyName"></param>
        /// <param name="componentName"></param>
        /// <returns></returns>
        public static JsonNode ReadPropertyFromDesired(JsonNode desired, string propertyName, string componentName)
        {
            JsonNode result = null;
            if (string.IsNullOrEmpty(componentName))
            {
                result = desired?[propertyName];
            }
            else
            {
                if (desired[componentName] != null &&
                    desired[componentName][propertyName] != null &&
                    desired[componentName]["__t"] != null &&
                    desired[componentName]["__t"].GetValue<string>() == "c")
                {
                    result = desired?[componentName][propertyName];
                }
            }

            return result;
        }
    }
}
