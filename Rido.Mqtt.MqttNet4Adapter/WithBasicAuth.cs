using MQTTnet.Client;
using Rido.MqttCore;
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
                .WithWillTopic($"pnp/{cs.ClientId}/lwt")
                .WithWillQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .WithWillPayload(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { lwtp = DateTime.Now})))
                .WithCredentials(cs.UserName, cs.Password);
            return builder;
        }
    }
}
