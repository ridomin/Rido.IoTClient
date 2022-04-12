using Rido.Mqtt.HubClient;
using System.Text.Json;

namespace layer2_sample
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
            //IMqttBaseClient adapter = await new MqttNet3Adapter.MqttNetClientConnectionFactory().CreateHubClientAsync(_configuration.GetConnectionString("cs"), stoppingToken);
            //IMqttBaseClient adapter = await new MqttNet4Adapter.MqttNetClientConnectionFactory().CreateHubClientAsync(_configuration.GetConnectionString("cs"),stoppingToken);
            //IMqttBaseClient adapter = await new M2MAdapter.M2MClientConnectionFactory().CreateHubClientAsync(_configuration.GetConnectionString("cs"), stoppingToken);
            //var client = new HubMqttClient(adapter);

            var client = await HubMqttClient.CreateFromConnectionStringAsync(_configuration.GetConnectionString("dps"), stoppingToken);
            _logger.LogInformation($"CONNECTED: DeviceId: {client.Connection.ConnectionSettings.DeviceId} - HostName: {client.Connection.ConnectionSettings.HostName} ");
            _logger.LogInformation($"Using MQTT Library:" + client.Connection.BaseClientLibraryInfo);

            var v = await client.ReportPropertyAsync(new { started = DateTime.Now }, stoppingToken);
            _logger.LogInformation($"Property updated with version {v}");
            
            var twin = await client.GetTwinAsync(stoppingToken);
            _logger.LogInformation(twin);

            client.OnCommandReceived = async m =>
            {
                _logger.LogInformation("Processing command: " + m.CommandName);
                return await Task.FromResult(new CommandResponse()
                {
                    Status = 200,
                    ReponsePayload = JsonSerializer.Serialize(new { myResponse = "whatever" })
                });
            };

            client.OnPropertyUpdateReceived = async m =>
            {
                _logger.LogInformation("Processing desired: " + m.ToJsonString());
                return await Task.FromResult(new GenericPropertyAck
                {
                    Value = m.ToJsonString(),
                    Status = 200,
                    Version = m["$version"].GetValue<int>()
                });
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                var puback = await client.SendTelemetryAsync(new { workingSet = Environment.WorkingSet }, stoppingToken);
                _logger.LogInformation($"Telemetry pubAck {puback}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
