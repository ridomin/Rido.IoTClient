using System;
using System.Text;

namespace Rido.IoTClient.AzBroker
{
    internal class SasAuth
    {
        const string apiversion_2021_06_30_preview = "2021-06-30-preview";
        internal static string GetUserName(string hostName, string deviceId, string moduleId, string expiryString, string modelId, string auth = "SAS")
        {
            string username = $"av={apiversion_2021_06_30_preview}&h={hostName}&did={deviceId}&am={auth}";
            if (!string.IsNullOrEmpty(moduleId))
            {
                username += $"&mid={moduleId}";
            }
            if (!string.IsNullOrEmpty(modelId))
            {
                username += $"&dtmi={modelId}";
            }
            if (auth == "SAS")
            {
                username += $"&se={expiryString}";
            }
            return username;
        }

        static byte[] CreateSasToken(string resource, string sasKey, string expiry)
        {
            static byte[] Sign(string requestString, string key)
            {
                using var algorithm = new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(key));
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(requestString));
            }
            return Sign($"{resource}\n\n\n{expiry}\n", sasKey);
        }

        internal static (string username, byte[] password) GenerateHubSasCredentials(string hostName, string deviceId, string sasKey, string modelId, int minutes)
        {
            var expiry = DateTimeOffset.UtcNow.AddMinutes(minutes).ToUnixTimeMilliseconds().ToString();
            string username = GetUserName(hostName, deviceId, string.Empty, expiry, modelId);
            byte[] password = CreateSasToken($"{hostName}\n{deviceId}", sasKey, expiry);
            return (username, password);
        }

        internal static (string username, byte[] password) GenerateHubSasCredentials(string hostName, string deviceId, string moduleId, string sasKey, string modelId, int minutes)
        {
            var expiry = DateTimeOffset.UtcNow.AddMinutes(minutes).ToUnixTimeMilliseconds().ToString();
            string username = GetUserName(hostName, deviceId, moduleId, expiry, modelId);
            byte[] password = CreateSasToken($"{hostName}\n{deviceId}/{moduleId}", sasKey, expiry);
            return (username, password);
        }
    }
}
