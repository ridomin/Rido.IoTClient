using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Rido.MqttCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Rido.Mqtt.MqttNet3Adapter
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
                .WithWillMessage(
                    new MqttApplicationMessageBuilder()
                        .WithTopic($"pnp/{cs.ClientId}/lwt")
                        .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                        .WithPayload(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { lwtp = DateTime.Now })))
                        .Build()
                )
                .WithCredentials(cs.UserName, cs.Password);
            return builder;
        }
    }
}