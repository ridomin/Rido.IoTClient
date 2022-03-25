
using Rido.PnP;

namespace pnp_device_sample
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
            var client = await dtmi_rido_pnp_memmon.CreateAsync(_configuration.GetConnectionString("local"), stoppingToken);

            //var twin = await client.GetTwinAsync(stoppingToken);
            //_logger.LogInformation(twin);

            client.Command_getRuntimeStats.OnCmdDelegate = async cmd =>
            {
                _logger.LogInformation("CMD getRuntimeStats");
                await Task.Delay(500);
                return new Cmd_getRuntimeStats_Response
                {
                    diagnosticResults = new Dictionary<string, string>()
                    {
                        { "machineName", Environment.MachineName },
                        { "osVersion", Environment.OSVersion.ToString() }
                    },
                    Status = 200
                };
            };

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

            client.Property_interval.PropertyValue = new PropertyAck<int>(client.Property_interval.PropertyName)
            {
                Value = 5,
                Status = 203,
                Description = "default value"
            };
            //await client.Property_interval.InitPropertyAsync(twin, 2, stoppingToken);
            //await client.Property_enabled.InitPropertyAsync(twin, true, stoppingToken);

            client.Property_started.PropertyValue = DateTime.Now;
            await client.Property_started.ReportPropertyAsync(stoppingToken);


            while (!stoppingToken.IsCancellationRequested)
            {
                await client.Telemetry_workingSet.SendTelemetryAsync(Environment.WorkingSet, stoppingToken);
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation($"enabled: {client.Property_enabled.PropertyValue.Value} {client.Property_enabled.PropertyValue.Version } ");
                _logger.LogInformation($"interval: {client.Property_interval.PropertyValue.Value} {client.Property_interval.PropertyValue.Version } ");
                _logger.LogInformation($"started: {client.Property_started.PropertyValue} {client.Property_started.Version } ");
                await Task.Delay(client.Property_interval.PropertyValue.Value * 1000, stoppingToken);
            }
        }
    }
}