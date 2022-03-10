using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rido.Mqtt.HubClient;
using Rido.Mqtt.M2MAdapter;
using Rido.Mqtt.MqttNetAdapter;
using Rido.MqttCore;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.MqttNetSample
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
            //IMqttBaseClient adapter = await new M2MClientConnectionFactory().CreateHubClientAsync(_configuration.GetConnectionString("cs"), stoppingToken);
            IMqttBaseClient adapter = await new MqttNetClientConnectionFactory().CreateHubClientAsync(_configuration.GetConnectionString("cs"),stoppingToken);
            
            _logger.LogInformation("Connected: " + adapter.ConnectionSettings.ToString());
            
            var client = new HubMqttClient(adapter);

            client.OnCommandReceived = async m =>
            {
                _logger.LogInformation("Processing command: " + m.CommandName);
                return await Task.FromResult(new GenericCommandResponse()
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

            var v = await client.ReportPropertyAsync(new { started = DateTime.Now }, stoppingToken);
            _logger.LogInformation($"Property updated with version {v}");

            var twin = await client.GetTwinAsync();

            _logger.LogInformation(twin);

            while (!stoppingToken.IsCancellationRequested)
            {
                int puback = await client.SendTelemetryAsync(new { temperature = 23 });
                _logger.LogInformation($"Telemetry pubAck {puback}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
