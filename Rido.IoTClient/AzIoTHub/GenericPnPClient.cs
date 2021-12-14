using MQTTnet.Client;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub
{
    public class GenericPnPClient : PnPClient
    {
        public GenericCommand Command;
        public GenericDesiredUpdatePropertyBinder genericDesiredUpdatePropertyBinder;
        public GenericPnPClient(IMqttClient c) : base(c)
        {
            Command = new GenericCommand(c);
            genericDesiredUpdatePropertyBinder = new GenericDesiredUpdatePropertyBinder(c);
        }

        public static new async Task<GenericPnPClient> CreateAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            var c = await PnPClient.CreateAsync(cs, cancellationToken);
            return new GenericPnPClient(c.Connection) { ConnectionSettings = cs};
        }

        public Task<MqttClientPublishResult> SendTelemetryAsync(object payload, CancellationToken t) =>
            Connection.PublishAsync($"devices/{Connection.Options.ClientId}/messages/events/", payload, t);
        
    }
}
