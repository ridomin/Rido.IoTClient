using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Rido.MqttCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace Rido.Mqtt.MqttNet3Adapter
{
    public static class WithAwsX509CredentialsExtension
    {
        public static MqttClientOptionsBuilder WithAwsX509Credentials(this MqttClientOptionsBuilder builder, ConnectionSettings cs)
        {
            var segments = cs.X509Key.Split('|');
            var cert = new X509Certificate2(segments[0], segments[1], X509KeyStorageFlags.Exportable);
            Trace.TraceInformation($"Loaded cert {cert.Subject} {cert.Thumbprint}");
            builder
                .WithTcpServer(cs.HostName, 8883)
                .WithKeepAlivePeriod(new TimeSpan(0, 0, 0, 300))
                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
                .WithClientId(cs.ClientId)
                .WithCleanSession(true)
                .WithWillMessage(
                    new MqttApplicationMessageBuilder()
                        .WithTopic($"pnp/{cs.ClientId}/lwt")
                        .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                        .WithPayload(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { lwtp = DateTime.Now })))
                        .WithRetainFlag(false)
                        .Build())
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = true,
                    AllowUntrustedCertificates = false,
                    IgnoreCertificateChainErrors = false,
                    IgnoreCertificateRevocationErrors = false,
                    SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
                    Certificates = new List<X509Certificate> { cert },
                });
            return builder;
        }
    }
}
