using MQTTnet;
using MQTTnet.Client;
using Rido.MqttCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.MqttNetAdapter
{
    public class MqttNetClient : IMqttBaseClient
    {
        public bool IsConnected => client.IsConnected;

        public string ClientId => client.Options.ClientId;

        public event EventHandler<DisconnectEventArgs> OnMqttClientDisconnected;
        public event Func<MqttMessage, Task> OnMessage;

        MqttClient client;
        public MqttNetClient(MqttClient client)
        {
            this.client = client;
            this.client.ApplicationMessageReceivedAsync += async m =>
            {
                await OnMessage.Invoke(
                    new MqttMessage()
                    {
                        Topic = m.ApplicationMessage.Topic,
                        Payload = Encoding.UTF8.GetString(m.ApplicationMessage.Payload)
                    });
            };

            this.client.DisconnectedAsync += async d =>
            {
                OnMqttClientDisconnected.Invoke(client, new DisconnectEventArgs() { ReasonInfo = d.Reason.ToString() });
                await Task.Yield();
            };
        }

        public static async Task<IMqttBaseClient> CreateAsync(string connectionSettingsString, CancellationToken cancellationToken = default)
        {
            var cs = new ConnectionSettings(connectionSettingsString);
            MqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient();
            var connAck = await mqtt.ConnectAsync(new MqttClientOptionsBuilder().WithAzureIoTHubCredentials(cs).Build(), cancellationToken);
            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }
            return new MqttNetClient(mqtt);
        }

        public async Task<int> PublishAsync(string topic, string payload, int qos = 0, CancellationToken token = default)
        {


            var res = await client.PublishAsync(
                new MqttApplicationMessage() 
                { 
                    Topic = topic, 
                    Payload = Encoding.UTF8.GetBytes(payload) 
                }, 
                token);

            if (res.ReasonCode != MqttClientPublishReasonCode.Success)
            {
                throw new ApplicationException("Error publishing to " + topic);
            }
            return 0;
        }

        public async Task<int> SubscribeAsync(string topic, CancellationToken token = default)
        {
            var res = await client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter(topic).Build(), token);
            var errs = res.Items.ToList().Any(x => x.ResultCode > MqttClientSubscribeResultCode.GrantedQoS2);
            if (errs)
            {
                throw new ApplicationException("Error subscribing to " + topic);
            }
            return 0;
        }
    }
}
