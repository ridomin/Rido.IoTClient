using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Rido.MqttCore
{
    public class ConnectionSettings
    {
        private const int Default_SasMinutes = 60;
        private const int Default_RetryInterval = 5;
        private const int Default_MaxRetries = 10;

        public string IdScope { get; set; }
        public string HostName { get; set; }
        public string DeviceId { get; set; }
        public string ClientId { get; set; }
        public string SharedAccessKey { get; set; }
        public string X509Key { get; set; } //paht-to.pfx|pfxpwd
        public string ModelId { get; set; }
        public string ModuleId { get; set; }
        public string Auth { get; set; }
        public int SasMinutes { get; set; }
        public int RetryInterval { get; set; }
        public int MaxRetries { get; set; }
        public ConnectionSettings()
        {
            SasMinutes = Default_SasMinutes;
            RetryInterval = Default_RetryInterval;
            MaxRetries = Default_MaxRetries;
            Auth = "SAS";
        }
        public static ConnectionSettings FromConnectionString(string cs) => new ConnectionSettings(cs);
        public ConnectionSettings(string cs) => ParseConnectionString(cs);

        static string GetStringValue(IDictionary<string, string> dict, string propertyName, string defaultValue = "")
        {
            string result = defaultValue;
            if (dict.TryGetValue(propertyName, out string value))
            {
                result = value;
            }
            return result;
        }

        static int GetPositiveIntValueOrDefault(IDictionary<string, string> dict, string propertyName, int defaultValue)
        {
            int result = defaultValue;
            if (dict.TryGetValue(propertyName, out string stringValue))
            {
                if (int.TryParse(stringValue, out int intValue))
                {
                    result = intValue;
                }
            }
            return result;
        }

        private void ParseConnectionString(string cs)
        {
           

            IDictionary<string, string> map = cs.ToDictionary(';', '=');
            IdScope = GetStringValue(map, nameof(IdScope));
            HostName = GetStringValue(map, nameof(HostName));
            DeviceId = GetStringValue(map, nameof(DeviceId));
            ClientId = GetStringValue(map, nameof(ClientId));
            SharedAccessKey = GetStringValue(map, nameof(SharedAccessKey));
            ModuleId = GetStringValue(map, nameof(ModuleId));
            X509Key = GetStringValue(map, nameof(X509Key));
            ModelId = GetStringValue(map, nameof(ModelId));
            Auth = GetStringValue(map, nameof(Auth), "SAS");
            SasMinutes = GetPositiveIntValueOrDefault(map, nameof(SasMinutes), Default_SasMinutes);
            RetryInterval = GetPositiveIntValueOrDefault(map, nameof(RetryInterval), Default_RetryInterval);
            MaxRetries = GetPositiveIntValueOrDefault(map, nameof(MaxRetries), Default_MaxRetries);
        }

        static void AppendIfNotEmpty(StringBuilder sb, string name, string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                if (name.Contains("Key"))
                {
                    sb.Append($"{name}=***;");
                }
                else
                {
                    sb.Append($"{name}={val};");
                }
            }
        }

        public override string ToString()
        {
            

            var result = new StringBuilder();
            AppendIfNotEmpty(result, nameof(HostName), HostName);
            AppendIfNotEmpty(result, nameof(DeviceId), DeviceId);
            AppendIfNotEmpty(result, nameof(IdScope), IdScope);
            AppendIfNotEmpty(result, nameof(ModuleId), ModuleId);
            AppendIfNotEmpty(result, nameof(SharedAccessKey), SharedAccessKey);
            AppendIfNotEmpty(result, nameof(ModelId), ModelId);
            AppendIfNotEmpty(result, nameof(ClientId), ClientId);
            AppendIfNotEmpty(result, nameof(SasMinutes), SasMinutes.ToString());
            AppendIfNotEmpty(result, nameof(RetryInterval), RetryInterval.ToString());
            AppendIfNotEmpty(result, nameof(MaxRetries), MaxRetries.ToString());
            AppendIfNotEmpty(result, nameof(X509Key), X509Key);
            AppendIfNotEmpty(result, nameof(Auth), Auth);
            result.Remove(result.Length - 1, 1);
            return result.ToString();
        }
    }

    internal static class StringToDictionaryExtension
    {
        internal static IDictionary<string, string> ToDictionary(this string valuePairString, char kvpDelimiter, char kvpSeparator)
        {
            if (string.IsNullOrWhiteSpace(valuePairString))
            {
                return new Dictionary<string, string>();
            }

            IEnumerable<string[]> parts = new Regex($"(?:^|{kvpDelimiter})([^{kvpDelimiter}{kvpSeparator}]*){kvpSeparator}")
                .Matches(valuePairString)
                .Cast<Match>()
                .Select(m => new string[] {
                    m.Result("$1"),
                    valuePairString.Substring(
                        m.Index + m.Value.Length,
                        (m.NextMatch().Success ? m.NextMatch().Index : valuePairString.Length) - (m.Index + m.Value.Length))
                });

            if (!parts.Any() || parts.Any(p => p.Length != 2))
            {
                return new Dictionary<string, string>();
            }

            IDictionary<string, string> map = parts.ToDictionary(kvp => kvp[0], (kvp) => kvp[1], StringComparer.OrdinalIgnoreCase);

            return map;
        }
    }
}
