﻿using System;
using System.Text;

namespace Rido.MqttCore
{
    /// <summary>
    /// Generate HMAC token to authenticate to IoTHub
    /// </summary>
    public class SasAuth
    {
        private const string apiversion_2020_09_30 = "2020-09-30";
        public static string GetUserName(string hostName, string deviceId, string modelId = "") =>
            $"{hostName}/{deviceId}/?api-version={apiversion_2020_09_30}&model-id={modelId}";

        private static string Sign(string requestString, string key)
        {
            using (var algorithm = new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(key)))
            {
                return Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(requestString)));
            }
        }

        public static string CreateSasToken(string resource, string sasKey, int minutes)
        {

            var expiry = DateTimeOffset.UtcNow.AddMinutes(minutes).ToUnixTimeSeconds().ToString();
            var sig = System.Net.WebUtility.UrlEncode(Sign($"{resource}\n{expiry}", sasKey));
            return $"SharedAccessSignature sr={resource}&sig={sig}&se={expiry}";
        }

        /// <summary>
        /// Generates username and password to connect to IoTHub
        /// </summary>
        /// <param name="hostName">IoT Hub hostname</param>
        /// <param name="deviceId">IoT Hub device Id</param>
        /// <param name="sasKey">Device Shared Access Key</param>
        /// <param name="modelId">PnP ModelId</param>
        /// <param name="minutes">Sas Token expire in minutes</param>
        /// <returns></returns>
        public static (string username, string password) GenerateHubSasCredentials(string hostName, string deviceId, string sasKey, string modelId, int minutes = 60) =>
            (GetUserName(hostName, deviceId, modelId), CreateSasToken($"{hostName}/devices/{deviceId}", sasKey, minutes));
    }
}
