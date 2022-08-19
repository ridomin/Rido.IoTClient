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
        public static MqttClientOptionsBuilder WithBasicAuth(this MqttClientOptionsBuilder builder, ConnectionSettings cs, bool withLWT = true)
        {
            builder
                .WithTcpServer(cs.HostName, cs.TcpPort)
                .WithClientId(cs.ClientId)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(cs.KeepAliveInSeconds))
                .WithCleanSession(false)
                .WithCredentials(cs.UserName, cs.Password);

            if (withLWT)
            {
                builder
                .WithWillTopic(BirthConvention.BirthTopic(cs.ClientId))
                .WithWillQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .WithWillPayload(BirthConvention.LastWillPayload(cs.ModelId))
                .WithWillRetain(true);
            }

            if (cs.UseTls)
            {
                builder.WithTls(new MqttClientOptionsBuilderTlsParameters()
                {
                    UseTls = true
                });
            }
            return builder;
        }
    }
}