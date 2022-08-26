﻿using Rido.Mqtt.Client;
using Rido.MqttCore;
using Rido.MqttCore.PnP;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.Client.TopicBindings
{
    public class Telemetry<T> : ITelemetry<T>
    {
        readonly IMqttConnection connection;
        readonly string deviceId;
        readonly string moduleId;
        readonly string name;
        readonly string component;

        public Telemetry(IMqttConnection connection, string name, string component = "", string moduleId = "")
        {
            this.connection = connection;
            this.name = name;
            this.component = component;
            deviceId = connection.ClientId;
            this.moduleId = moduleId;
        }

        public async Task<int> SendTelemetryAsync(T payload, CancellationToken cancellationToken = default)
        {
            string topic = $"pnp/{deviceId}";

            if (!string.IsNullOrEmpty(component))
            {
                topic += $"/{component}";
            }
            if (!string.IsNullOrEmpty(moduleId))
            {
                topic += $"/modules/{moduleId}";
            }
            topic += "/telemetry";


            Dictionary<string, T> typedPayload = new Dictionary<string, T>
            {
                { name, payload }
            };
            return await connection.PublishAsync(topic, typedPayload, 0, false, cancellationToken);
        }
    }
}
