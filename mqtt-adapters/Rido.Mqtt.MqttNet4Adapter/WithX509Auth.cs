using MQTTnet.Client;
using Rido.MqttCore.Birth;
using Rido.MqttCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using MQTTnet.Implementations;

namespace Rido.Mqtt.MqttNet4Adapter
{
    public static partial class MqttNetExtensions
    {
        public static MqttClientOptionsBuilder WithX509Auth(this MqttClientOptionsBuilder builder, ConnectionSettings cs, bool withLWT = false)
        {
            var certs = new List<X509Certificate2>();

            if (!string.IsNullOrEmpty(cs.X509Key))
            {
                var cert = ClientCertificateLocator.Load(cs.X509Key);
                certs.Add(cert);
            }

            if (!string.IsNullOrEmpty(cs.Password))
            {
                builder.WithCredentials(cs.UserName, cs.Password);
            }

            var tls = new MqttClientOptionsBuilderTlsParameters();
            tls.UseTls = cs.UseTls;
            tls.IgnoreCertificateRevocationErrors = cs.DisableCrl;
            if (!string.IsNullOrEmpty(cs.CaPath))
            {
                var caCert = new X509Certificate2(cs.CaPath);
                //certs.Add(caCert);
                tls.CertificateValidationHandler = ea =>
                {
#if NET6_0
                    X509Chain chain = new X509Chain();
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
                    chain.ChainPolicy.VerificationTime = DateTime.Now;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 0);
                    chain.ChainPolicy.CustomTrustStore.Add(caCert);
                    chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                    var x5092 = new X509Certificate2(ea.Certificate);
                    return chain.Build(x5092);
#endif
#if NETSTANDARD2_0
                    return ea.Certificate.Issuer == caCert.Subject;
#endif
                };
            }
            
            tls.Certificates = certs;
            builder
                .WithTcpServer(cs.HostName, cs.TcpPort)
                .WithClientId(cs.ClientId)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(cs.KeepAliveInSeconds))
                .WithCleanSession(true)
                .WithTls(tls);

            if (withLWT)
            {
                builder
                .WithWillTopic(BirthConvention.BirthTopic(cs.ClientId))
                .WithWillQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .WithWillPayload(BirthConvention.LastWillPayload(cs.ModelId))
                .WithWillRetain(true);
            }
            return builder;
        }
    }
}

