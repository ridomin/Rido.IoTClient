using Rido.Mqtt.HubClient;

namespace pnp_device_sample
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = await dtmi_rido_pnp_memmon.CreateAsync(_configuration.GetConnectionString("cs"), stoppingToken);

            client.Property_interval.OnProperty_Updated += async p =>
            {
                await Task.Yield();
                _logger.LogInformation("Received property interval");
                client.Property_interval.PropertyValue.Value = p.Value;

                return new PropertyAck<int>(p.Name)
                {
                    Value = p.Value,
                    Status = 200,
                    Version = p.Version
                };
            }; 

            var twin = await client.GetTwinAsync();
            await client.Property_interval.InitPropertyAsync(twin, 2, stoppingToken);
            await client.Property_enabled.InitPropertyAsync(twin, true, stoppingToken);

            client.Property_started.PropertyValue = DateTime.Now;
            await client.Property_started.ReportPropertyAsync(stoppingToken);


            while (!stoppingToken.IsCancellationRequested)
            {
                await client.Telemetry_workingSet.SendTelemetryAsync(3.2, stoppingToken);
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation($"enabled: {client.Property_enabled.PropertyValue.Value} {client.Property_enabled.PropertyValue.Version } ");
                _logger.LogInformation($"interval: {client.Property_interval.PropertyValue.Value} {client.Property_interval.PropertyValue.Version } ");
                _logger.LogInformation($"started: {client.Property_started.PropertyValue} {client.Property_started.Version } ");
                await Task.Delay(client.Property_interval.PropertyValue.Value * 1000, stoppingToken);
            }
        }
    }
}