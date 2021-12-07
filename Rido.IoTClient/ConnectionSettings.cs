using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Rido.IoTClient
{
    public class ConnectionSettings
    {
        const int Default_SasMinutes = 60;
        const int Default_RetryInterval = 5;
        const int Default_MaxRetries = 10;

        public string IdScope { get; set; }
        public string HostName { get; set; }
        public string DeviceId { get; set; }
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
            this.SasMinutes = Default_SasMinutes;
            this.RetryInterval = Default_RetryInterval;
            this.MaxRetries = Default_MaxRetries;
            this.Auth = "SAS";
        }
        public static ConnectionSettings FromConnectionString(string cs) => new ConnectionSettings(cs);
        public ConnectionSettings(string cs) => ParseConnectionString(cs);

        private void ParseConnectionString(string cs)
        {
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

            IDictionary<string, string> map = cs.ToDictionary(';', '=');
            this.IdScope = GetStringValue(map, nameof(this.IdScope));
            this.HostName = GetStringValue(map, nameof(this.HostName));
            this.DeviceId = GetStringValue(map, nameof(this.DeviceId));
            this.SharedAccessKey = GetStringValue(map, nameof(this.SharedAccessKey));
            this.ModuleId = GetStringValue(map, nameof(this.ModuleId));
            this.X509Key = GetStringValue(map, nameof(this.X509Key));
            this.ModelId = GetStringValue(map, nameof(this.ModelId));
            this.Auth = GetStringValue(map, nameof(this.Auth), "SAS");
            this.SasMinutes = GetPositiveIntValueOrDefault(map, nameof(this.SasMinutes), Default_SasMinutes);
            this.RetryInterval = GetPositiveIntValueOrDefault(map, nameof(this.RetryInterval), Default_RetryInterval);
            this.MaxRetries = GetPositiveIntValueOrDefault(map, nameof(this.MaxRetries), Default_MaxRetries);
        }

        public override string ToString()
        {
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

            var result = new StringBuilder();
            AppendIfNotEmpty(result, nameof(this.HostName), HostName);
            AppendIfNotEmpty(result, nameof(this.DeviceId), DeviceId);
            AppendIfNotEmpty(result, nameof(this.IdScope), IdScope);
            AppendIfNotEmpty(result, nameof(this.ModuleId), ModuleId);
            AppendIfNotEmpty(result, nameof(this.SharedAccessKey), SharedAccessKey);
            AppendIfNotEmpty(result, nameof(this.ModelId), ModelId);
            AppendIfNotEmpty(result, nameof(this.SasMinutes), SasMinutes.ToString());
            AppendIfNotEmpty(result, nameof(this.RetryInterval), RetryInterval.ToString());
            AppendIfNotEmpty(result, nameof(this.MaxRetries), MaxRetries.ToString());
            AppendIfNotEmpty(result, nameof(this.X509Key), X509Key);
            AppendIfNotEmpty(result, nameof(this.Auth), Auth);
            result.Remove(result.Length - 1, 1);
            return result.ToString();
        }
    }

    static class StringToDictionaryExtension
    {
        internal static IDictionary<string, string> ToDictionary(this string valuePairString, char kvpDelimiter, char kvpSeparator)
        {
            if (string.IsNullOrWhiteSpace(valuePairString))
            {
                return null;
            }

            IEnumerable<string[]> parts = new Regex($"(?:^|{kvpDelimiter})([^{kvpDelimiter}{kvpSeparator}]*){kvpSeparator}")
                .Matches(valuePairString)
                .Cast<Match>()
                .Select(m => new string[] {
                    m.Result("$1"),
                    valuePairString[
                        (m.Index + m.Value.Length)..(m.NextMatch().Success ? m.NextMatch().Index : valuePairString.Length)]
                });

            if (!parts.Any() || parts.Any(p => p.Length != 2))
            {
                return null;
            }

            IDictionary<string, string> map = parts.ToDictionary(kvp => kvp[0], (kvp) => kvp[1], StringComparer.OrdinalIgnoreCase);

            return map;
        }
    }
}


