using MQTTnet.Client;
using Rido.MqttCore;
using Rido.MqttCore.Birth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Rido.Mqtt.MqttNet4Adapter
{
    public static partial class MqttNetExtensions
    {
        public static MqttClientOptionsBuilder WithBasicAuth(this MqttClientOptionsBuilder builder, ConnectionSettings cs)
        {
            builder
             .WithTcpServer(cs.HostName, 8883)
                .WithTls(new MqttClientOptionsBuilderTlsParameters()
                {
                    UseTls = true,
                    IgnoreCertificateRevocationErrors = true
                })
                .WithClientId(cs.ClientId)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(cs.KeepAliveInSeconds))
                .WithCleanSession(false)
                .WithWillTopic(BirthConvention.BirthTopic(cs.ClientId))
                .WithWillQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .WithWillPayload(BirthConvention.LastWillPayload(cs.ModelId))
                .WithWillRetain(true)
                .WithCredentials(cs.UserName, cs.Password);
            return builder;
        }
    }
}