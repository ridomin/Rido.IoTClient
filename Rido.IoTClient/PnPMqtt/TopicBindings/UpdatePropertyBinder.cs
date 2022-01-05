using MQTTnet.Client;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.PnPMqtt.TopicBindings
{
    public class UpdatePropertyBinder : IReportPropertyBinder
    {
        readonly IMqttClient connection;

        public UpdatePropertyBinder(IMqttClient connection)
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
