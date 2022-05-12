using Rido.MqttCore.PnP;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Rido.HubProxy
{
    public class TwinProperty<T>
    {
        public static PropertyAck<T> InitFromTwin(string twinJson, string propName, string componentName, T defaultValue)
        {
            if (string.IsNullOrEmpty(twinJson))
            {
                return new PropertyAck<T>(propName, componentName) { Value = defaultValue };
            }

            var root0 = JsonNode.Parse(twinJson);
            var root = root0["properties"];
            var desired = root?["desired"];
            var reported = root?["reported"];
            T desired_Prop = default;
            int desiredVersion = desired["$version"].GetValue<int>();
            PropertyAck<T> result = new PropertyAck<T>(propName, componentName) { DesiredVersion = desiredVersion };

            bool desiredFound = false;
            if (!string.IsNullOrEmpty(componentName))
            {
                if (desired[componentName] != null &&
                    desired[componentName]["__t"] != null &&
                    desired[componentName]["__t"]?.GetValue<string>() == "c" &&
                    desired[componentName][propName] != null)
                {
                    desired_Prop = desired[componentName][propName].Deserialize<T>();
                    desiredFound = true;
                }
            }
            else
            {
                if (desired[propName] != null)
                {
                    desired_Prop = desired[propName].Deserialize<T>();
                    desiredFound = true;
                }
            }

            bool reportedFound = false;
            T reported_Prop = default;
            int reported_Prop_version = 0;
            int reported_Prop_status = 001;
            string reported_Prop_description = string.Empty;

            if (!string.IsNullOrEmpty(componentName))
            {
                if (reported[componentName] != null &&
                    reported[componentName]["__t"]?.GetValue<string>() == "c" &&
                    reported[componentName][propName] != null)
                {
                    reported_Prop = reported[componentName][propName]["value"].Deserialize<T>();
                    reported_Prop_version = reported[componentName][propName]["av"]?.GetValue<int>() ?? -1;
                    reported_Prop_status = reported[componentName][propName]["ac"].GetValue<int>();
                    reported_Prop_description = reported[componentName][propName]["ad"]?.GetValue<string>();
                    reportedFound = true;
                }
            }
            else
            {
                if (reported[propName] != null)
                {
                    reported_Prop = reported[propName]["value"].Deserialize<T>();

                    reported_Prop_version = reported[propName]["av"]?.GetValue<int>() ?? -1;
                    reported_Prop_status = reported[propName]["ac"].GetValue<int>();
                    reported_Prop_description = reported[propName]["ad"]?.GetValue<string>();
                    reportedFound = true;
                }
            }

            if (!desiredFound && !reportedFound)
            {
                result = new PropertyAck<T>(propName, componentName)
                {
                    //DesiredVersion = desiredVersion,
                    Version = reported_Prop_version,
                    Value = defaultValue,
                    Status = 203,
                    Description = "Init from default value"
                };
            }

            if (!desiredFound && reportedFound)
            {
                result = new PropertyAck<T>(propName, componentName)
                {
                    DesiredVersion = 0,
                    Version = reported_Prop_version,
                    Value = reported_Prop,
                    Status = reported_Prop_status,
                    Description = reported_Prop_description
                };
            }

            if (desiredFound && reportedFound)
            {
                if (desiredVersion >= reported_Prop_version)
                {
                    result = new PropertyAck<T>(propName, componentName)
                    {
                        DesiredVersion = desiredVersion,
                        Value = desired_Prop,
                        Version = desiredVersion
                    };
                }
            }


            if (desiredFound && !reportedFound)
            {
                result = new PropertyAck<T>(propName, componentName)
                {
                    DesiredVersion = desiredVersion,
                    Version = desiredVersion,
                    Value = desired_Prop
                };
            }
            return result;
        }
    }
}
