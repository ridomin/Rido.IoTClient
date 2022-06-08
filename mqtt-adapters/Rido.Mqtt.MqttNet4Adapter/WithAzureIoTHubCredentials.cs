using MQTTnet.Client;
using Rido.MqttCore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Rido.Mqtt.MqttNet4Adapter
{
    public static partial class MqttNetExtensions
    {
        public static MqttClientOptionsBuilder WithAzureIoTHubCredentials(this MqttClientOptionsBuilder builder, ConnectionSettings cs)
        {
            if (cs.Auth == AuthType.Sas)
            {
                cs.ClientId = cs.DeviceId;
                return builder.WithAzureIoTHubCredentialsSas(cs.HostName, cs.DeviceId, cs.ModuleId, cs.SharedAccessKey, cs.ModelId, cs.SasMinutes);
            }
            else if (cs.Auth == AuthType.X509)
            {
                var cert = ClientCertificateLocator.Load(cs.X509Key);
                string clientId = X509CommonNameParser.GetCNFromCertSubject(cert.Subject);
                if (clientId.Contains('/')) //is a module
                {
                    var segmentsId = clientId.Split('/');
                    cs.DeviceId = segmentsId[0];
                    cs.ModuleId = segmentsId[1];
                }
                else
                {
                    cs.DeviceId = clientId;
                }

                return builder.WithAzureIoTHubCredentialsX509(cs.HostName, cert, cs.ModelId);
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
                (string username, string password) = SasAuth.GenerateHubSasCredentials(hostName, deviceId, sasKey, modelId, sasMinutes);
                builder
                    .WithTcpServer(hostName, 8883)
                    .WithTls()
                    .WithClientId(deviceId)
                    .WithCredentials(username, password);
            }
            else
            {
                (string username, string password) = SasAuth.GenerateHubSasCredentials(hostName, $"{deviceId}/{moduleId}", sasKey, modelId, sasMinutes);
                builder
                    .WithTcpServer(hostName, 8883)
                    .WithTls()
                    .WithClientId($"{deviceId}/{moduleId}")
                    .WithCredentials(username, password);
            }
            return builder;
        }

        public static MqttClientOptionsBuilder WithAzureIoTHubCredentialsX509(this MqttClientOptionsBuilder builder, string hostName, X509Certificate cert, string modelId)
        {
            string clientId = X509CommonNameParser.GetCNFromCertSubject(cert.Subject);

            builder
                .WithTcpServer(hostName, 8883)
                .WithClientId(clientId)
                .WithCredentials(new MqttClientCredentials(SasAuth.GetUserName(hostName, clientId, modelId)))
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
