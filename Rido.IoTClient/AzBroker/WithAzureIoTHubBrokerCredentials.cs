using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Rido.IoTClient.AzBroker
{
    public static class MqttNetExtensions
    {
        public static MqttClientOptionsBuilder WithAzureIoTHubBrokerCredentials(this MqttClientOptionsBuilder builder, ConnectionSettings cs)
        {
            if (cs.Auth == "SAS")
            {
                return WithAzureIoTHubCredentialsSas(builder, cs.HostName, cs.DeviceId, cs.ModuleId, cs.SharedAccessKey, cs.ModelId, cs.SasMinutes);
            }
            else if (cs.Auth == "X509")
            {
                var segments = cs.X509Key.Split('|');
                string pfxpath = segments[0];
                string pfxpwd = segments[1];
                var cert = new X509Certificate2(pfxpath, pfxpwd);
                var clientId = X509CommonNameParser.GetCNFromCertSubject(cert.Subject);
                if (clientId.Contains('/')) //is a module
                {
                    var segmentsId = clientId.Split('/');
                    cs.DeviceId = segmentsId[0];
                    cs.ModuleId = segmentsId[1];
                }
                else
                {
                    cs.ClientId = clientId;
                    cs.DeviceId = clientId;
                }

                return WithAzureIoTHubCredentialsX509(builder, cs.HostName, cs.DeviceId, cs.ModuleId, cert, cs.ModelId);
            }
            else
            {
                throw new ApplicationException("Auth not supported: " + cs.Auth);
            }
        }

        public static MqttClientOptionsBuilder WithAzureIoTHubCredentialsSas(this MqttClientOptionsBuilder builder, string hostName, string deviceId, string moduleId, string sasKey, string modelId, int sasMinutes)
        {
            if (string.IsNullOrEmpty(moduleId))
            {
                (string username, byte[] password) = SasAuth.GenerateHubSasCredentials(hostName, deviceId, sasKey, modelId, sasMinutes);
                builder
                    .WithTcpServer(hostName, 8883)
                    .WithTls()
                    .WithClientId(deviceId)
                    .WithCredentials(username, password);
            }
            else
            {
                (string username, byte[] password) = SasAuth.GenerateHubSasCredentials(hostName, $"{deviceId}/{moduleId}", sasKey, modelId, sasMinutes);
                builder
                    .WithTcpServer(hostName, 8883)
                    .WithTls()
                    .WithClientId($"{deviceId}/{moduleId}")
                    .WithCredentials(username, password);
            }
            return builder;
        }

        public static MqttClientOptionsBuilder WithAzureIoTHubCredentialsX509(this MqttClientOptionsBuilder builder, string hostName, string clientId, string moduleId, X509Certificate cert, string modelId)
        {
            builder
                .WithTcpServer(hostName, 8883)
                .WithClientId(clientId)
                .WithCredentials(new MqttClientCredentials()
                {
                    Username = SasAuth.GetUserName(hostName, clientId, moduleId, string.Empty, modelId, "X509")
                })
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = true,
                    SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
                    Certificates = new List<X509Certificate> { cert }
                });
            return builder;
        }
    }
}
