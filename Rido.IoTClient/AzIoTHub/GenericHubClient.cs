using MQTTnet.Client;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub
{
    public class GenericHubClient : IoTHubClient
    {
        public GenericCommand Command;
        public GenericDesiredUpdatePropertyBinder genericDesiredUpdateProperty;
        
        public GenericHubClient(IMqttClient c) : base(c)
        {   
            Command = new GenericCommand(c);
            genericDesiredUpdateProperty = new GenericDesiredUpdatePropertyBinder(c);
        }

        public Task<MqttClientPublishResult> SendTelemetryAsync(object payload, CancellationToken t = default) =>
            Connection.PublishAsync($"devices/{Connection.Options.ClientId}/messages/events/", payload, t);

    }
}
