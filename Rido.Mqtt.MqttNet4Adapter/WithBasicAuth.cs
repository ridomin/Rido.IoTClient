﻿using MQTTnet.Client;
using Rido.MqttCore;
using System;
using System.Collections.Generic;
using System.Text;

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
                .WithCredentials(cs.UserName, cs.Password);
            return builder;
        }
    }
}