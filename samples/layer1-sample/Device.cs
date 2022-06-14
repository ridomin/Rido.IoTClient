using MQTTnet.Client;
using Rido.MqttCore;

namespace layer1_sample
{
    public class Device : BackgroundService
    {
        private readonly ILogger<Device> _logger;
        private readonly IConfiguration _configuration;

        private int reconnects = 0;

        public Device(ILogger<Device> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IMqttBaseClient mqtt = await new Rido.Mqtt.MqttNet4Adapter.MqttNetClientConnectionFactory()
                .CreateHubClientAsync(_configuration.GetConnectionString("cs"), stoppingToken);

            mqtt.OnMqttClientDisconnected += Mqtt_OnMqttClientDisconnected;

            Console.WriteLine($"{mqtt.BaseClientLibraryInfo} {mqtt.ConnectionSettings}");

            await mqtt.PublishAsync(
                $"$iothub/twin/PATCH/properties/reported/?$rid=1", 
                new { DateTime.Now }, 1, false, stoppingToken);

            var twin = await GetTwin(mqtt, stoppingToken);
            Console.WriteLine(twin);
            int numMsg = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                var pubAck = await mqtt.PublishAsync(
                    $"devices/{mqtt.ConnectionSettings.ClientId}/messages/events/",
                    new { worksingSet = Environment.WorkingSet },
                    1, false, stoppingToken);

                if (pubAck == 0)
                {
                    numMsg++;
                }

                _logger.LogInformation("Messages sent {numMsg}, reconnects {reconnects}", numMsg, reconnects);
                
                await Task.Delay(10000, stoppingToken);
            }
        }

        private void Mqtt_OnMqttClientDisconnected(object? sender, DisconnectEventArgs e)
        {
            Console.WriteLine($"Client Disconnected reason: {e.ReasonInfo} reconnects: {reconnects++}");
        }

        int rid = 0;
        async Task<string> GetTwin(IMqttBaseClient client, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            _ = await client.SubscribeAsync("$iothub/twin/res/#", cancellationToken);
            client.OnMessage += async m =>
            {
                var topic = m.Topic;

                if (topic.StartsWith("$iothub/twin/res/200"))
                {
                    tcs.SetResult(m.Payload);
                    _ = await client.UnsubscribeAsync("$iothub/twin/res/#", cancellationToken);
                }
                await Task.Yield();
            };
            var puback = await client.PublishAsync($"$iothub/twin/GET/?$rid={rid++}", string.Empty, 1, false, cancellationToken);
            return await tcs.Task.TimeoutAfter(TimeSpan.FromSeconds(10));
        }
    }
}