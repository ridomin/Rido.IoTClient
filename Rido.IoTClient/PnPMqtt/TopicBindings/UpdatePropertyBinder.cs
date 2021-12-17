﻿using MQTTnet.Client;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.PnPMqtt.TopicBindings
{
    public class UpdatePropertyBinder : IReportPropertyBinder
    {
        readonly IMqttClient connection;

        private static UpdatePropertyBinder instance;

        public static UpdatePropertyBinder GetInstance(IMqttClient connection)
        {
            if (instance == null || instance.connection != connection)
            {
                instance = new UpdatePropertyBinder(connection);
            }
            return instance;

        }
        private UpdatePropertyBinder(IMqttClient connection)
        {
            this.connection = connection;
        }

        public async Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default)
        {
            var puback = await connection.PublishAsync($"pnp/{connection.Options.ClientId}/props/reported", payload, cancellationToken);
            if (puback.ReasonCode == MqttClientPublishReasonCode.Success)
            {
                return 0;
            }
            return -1;
        }


    }
}