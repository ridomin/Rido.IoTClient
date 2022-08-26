﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Rido.MqttCore
{
    public enum AuthType
    {
        Sas,
        X509,
        Basic
    }

    /// <summary>
    /// Allows to configure MQTT connections for a KeyValue string
    /// </summary>
    public class ConnectionSettings
    {
        private const int Default_SasMinutes = 60;
        private const int Default_KeepAliveInSeconds = 60;
        private const string Default_CleanSession = "true";
        private const int Default_TcpPort = 8883;
        private const string Default_UseTls = "true";

        /// <summary>
        /// Id Scope from Azure IoT Hub DPS service
        /// </summary>
        public string IdScope { get; set; }
        /// <summary>
        /// MQTT/IoTHub host name
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// Device for Azure IoT Hub/DPS clients
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// MQTT Client Id, when empty will use the machine name
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Azure IoT Hub/DPS Shared Access Key
        /// </summary>
        public string SharedAccessKey { get; set; }
        /// <summary>
        /// X509 Key in one of this forms:
        /// - path-to-pfx|pfxpwd
        /// - thumbprint (private key must exist in the CurrentUser cert store)
        /// </summary>
        public string X509Key { get; set; } //paht-to.pfx|pfxpwd, or thumbprint
        /// <summary>
        /// PnP Model Id to announce during the connection
        /// </summary>
        public string ModelId { get; set; }
        /// <summary>
        /// Azure IoT Hub Module Id
        /// </summary>
        public string ModuleId { get; set; }
        /// <summary>
        /// Authentication type, inferred from current settings
        /// </summary>
        public AuthType Auth { get; set; }
        /// <summary>
        /// Time in minutes to refresh the Sas Token used to authenticate to IoT Hub
        /// </summary>
        public int SasMinutes { get; set; }
        /// <summary>
        /// MQTT Basic Auth user name
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// MQTT Basic Auth password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Keep alive ping refresh in seconds
        /// </summary>
        public int KeepAliveInSeconds { get; set; }
        /// <summary>
        /// Open MQTT connection with clean session, defaults to true
        /// </summary>
        public bool CleanSession { get; set; }
        /// <summary>
        /// Tcp Port to establish the connection, defaults to 8883
        /// </summary>
        public int TcpPort { get; set; }
        /// <summary>
        /// Enable Server TLS connections, defaults to true
        /// </summary>
        public bool UseTls { get; set; }

        public ConnectionSettings()
        {
            SasMinutes = Default_SasMinutes;
            Auth = AuthType.Basic;
            TcpPort = Default_TcpPort;
            UseTls = Default_UseTls == "true";
        }
        public static ConnectionSettings FromConnectionString(string cs) => new ConnectionSettings(cs);
        public ConnectionSettings(string cs) => ParseConnectionString(cs);

        private static string GetStringValue(IDictionary<string, string> dict, string propertyName, string defaultValue = "")
        {
            string result = defaultValue;
            if (dict.TryGetValue(propertyName, out string value))
            {
                result = value;
            }
            return result;
        }

        private static int GetPositiveIntValueOrDefault(IDictionary<string, string> dict, string propertyName, int defaultValue)
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
            ClientId = GetStringValue(map, nameof(ClientId), Environment.MachineName);
            SharedAccessKey = GetStringValue(map, nameof(SharedAccessKey));
            ModuleId = GetStringValue(map, nameof(ModuleId));
            X509Key = GetStringValue(map, nameof(X509Key));
            ModelId = GetStringValue(map, nameof(ModelId));
            //Auth = GetStringValue(map, nameof(Auth), "SAS");
            SasMinutes = GetPositiveIntValueOrDefault(map, nameof(SasMinutes), Default_SasMinutes);
            UserName = GetStringValue(map, nameof(UserName));
            Password = GetStringValue(map, nameof(Password));
            KeepAliveInSeconds = GetPositiveIntValueOrDefault(map, nameof(KeepAliveInSeconds), Default_KeepAliveInSeconds);
            CleanSession = GetStringValue(map, nameof(CleanSession), Default_CleanSession) == "true";
            TcpPort = GetPositiveIntValueOrDefault(map, nameof(TcpPort), Default_TcpPort);
            UseTls = GetStringValue(map, nameof(UseTls), Default_UseTls) == "true";
            
            //if (string.IsNullOrEmpty(SharedAccessKey) && string.IsNullOrEmpty(X509Key) && string.IsNullOrEmpty(Password))
            //{
            //    throw new KeyNotFoundException("ConnectionString does not have any key");
            //}

            if (!String.IsNullOrEmpty(X509Key))
            {
                Auth = AuthType.X509;
            }

            if (!String.IsNullOrEmpty(SharedAccessKey))
            {
                Auth = AuthType.Sas;
            }

            if (!String.IsNullOrEmpty(Password))
            {
                Auth = AuthType.Basic;
            }

        }

        private static void AppendIfNotEmpty(StringBuilder sb, string name, string val)
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
            AppendIfNotEmpty(result, nameof(X509Key), X509Key);
            AppendIfNotEmpty(result, nameof(Auth), Auth.ToString());
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
