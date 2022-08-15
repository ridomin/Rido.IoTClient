using MQTTnet;
using MQTTnet.Client.Options;
using Rido.MqttCore;
using Rido.MqttCore.Birth;
using System;

namespace Rido.Mqtt.MqttNet3Adapter
{
    public static partial class MqttNetExtensions
    {
        public static MqttClientOptionsBuilder WithBasicAuth(this MqttClientOptionsBuilder builder, ConnectionSettings cs)
        {
            builder
             .WithTcpServer(cs.HostName, cs.TcpPort)

                .WithClientId(cs.ClientId)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(cs.KeepAliveInSeconds))
                .WithCleanSession(false)
                .WithWillMessage(
                    new MqttApplicationMessageBuilder()
                        .WithTopic(BirthConvention.BirthTopic(cs.ClientId))
                        .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                        .WithPayload(BirthConvention.LastWillPayload())
                        .WithRetainFlag(true)
                        .Build()
                )
                .WithCredentials(cs.UserName, cs.Password);

            if (cs.UseTls)
            {
                builder.WithTls(new MqttClientOptionsBuilderTlsParameters()
                {
                    UseTls = true,
                    IgnoreCertificateRevocationErrors = true
                });
            }
            return builder;
        }
    }
}