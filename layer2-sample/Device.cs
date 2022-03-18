using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rido.Mqtt.HubClient;
using Rido.MqttCore;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.MqttNetSample
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

            var client = await HubMqttClient.CreateFromConnectionStringAsync(_configuration.GetConnectionString("cs"));
            _logger.LogInformation($"CONNECTED: DeviceId: {client.Connection.ConnectionSettings.DeviceId} - HostName: {client.Connection.ConnectionSettings.HostName} ");
            _logger.LogInformation($"Using MQTT Library:" + client.Connection.BaseClientLibraryInfo);

            var v = await client.ReportPropertyAsync(new { started = DateTime.Now }, stoppingToken);
            _logger.LogInformation($"Property updated with version {v}");
            
            var twin = await client.GetTwinAsync();
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
                var puback = await client.SendTelemetryAsync(new { workingSet = Environment.WorkingSet });
                _logger.LogInformation($"Telemetry pubAck {puback}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
