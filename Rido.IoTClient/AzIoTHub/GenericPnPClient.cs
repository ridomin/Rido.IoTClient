using MQTTnet.Client;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub
{
    public class GenericPnPClient : IoTHubPnPClient
    {
        public GenericCommand Command;
        public GenericDesiredUpdatePropertyBinder genericDesiredUpdatePropertyBinder;
        
        public GenericPnPClient(IMqttClient c) : base(c)
        {   
            Command = new GenericCommand(c);
            genericDesiredUpdatePropertyBinder = new GenericDesiredUpdatePropertyBinder(c);
        }

        public Task<MqttClientPublishResult> SendTelemetryAsync(object payload, CancellationToken t = default) =>
            Connection.PublishAsync($"devices/{Connection.Options.ClientId}/messages/events/", payload, t);

    }
}
