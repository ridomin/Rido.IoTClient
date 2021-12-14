using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;

namespace Rido.IoTClient
{
    internal class TwinParser
    {
        internal static JsonNode ReadPropertyFromDesired(JsonNode desired, string propertyName, string componentName)
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

                    result = desired?[componentName][propertyName];
            }

            return result;
        }
    }
}
