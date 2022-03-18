using Rido.Mqtt.MqttNet3Adapter;
using Rido.MqttCore;

namespace layer1_sample
{
    public class Device : BackgroundService
    {
        private readonly ILogger<Device> _logger;
        private readonly IConfiguration _configuration;

        public Device(ILogger<Device> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IMqttBaseClient mqtt = await new MqttNetClientConnectionFactory()
                .CreateHubClientAsync(_configuration.GetConnectionString("cs"), stoppingToken);
            _logger.LogInformation($"{mqtt.BaseClientLibraryInfo} {mqtt.ConnectionSettings}");

            var twin = await GetTwin(mqtt, stoppingToken);
            _logger.LogInformation(twin);

            while (!stoppingToken.IsCancellationRequested)
            {
                var pubAck = await mqtt.PublishAsync(
                    $"devices/{mqtt.ConnectionSettings.ClientId}/messages/events/",
                    new { worksingSet = Environment.WorkingSet },
                    1, stoppingToken);

                _logger.LogInformation($"PubAck: {pubAck}");
                await Task.Delay(1000, stoppingToken);
            }
        }

        int rid = 0;
        async Task<string> GetTwin(IMqttBaseClient client, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            var subAck = await client.SubscribeAsync("$iothub/twin/res/#");
            client.OnMessage += async m =>
            {
                var topic = m.Topic;

                if (topic.StartsWith("$iothub/twin/res/200"))
                {
                    tcs.SetResult(m.Payload);
                }
                await Task.Yield();
            };
            var puback = await client.PublishAsync($"$iothub/twin/GET/?$rid={rid++}", string.Empty, 1, cancellationToken);
            return await tcs.Task;

        }
            
    }
}