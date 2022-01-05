using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace pnp_generic_client
{
    public class DeviceRunner : BackgroundService
    {
        private readonly ILogger<DeviceRunner> _logger;
        private readonly IConfiguration _configuration;

        public DeviceRunner(ILogger<DeviceRunner> logger, IConfiguration config)
        {
            _logger = logger;
            _configuration = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            var cs = new ConnectionSettings(_configuration.GetConnectionString("cs")) { ModelId = "dtmi:tartabit:AssetTracker;1" };
            var mqtt= await IoTHubConnectionFactory.CreateAsync(cs, stoppingToken);
            var client = new GenericPnPClient(mqtt) { ConnectionSettings = cs };

            _logger.LogInformation($"Connected to {client.ConnectionSettings}");

            await client.ReportPropertyAsync(new { started = DateTime.Now }, stoppingToken);
            var twin = await client.GetTwinAsync(stoppingToken);

            client.Command.OnCmdDelegate = async m =>
            {
                _logger.LogInformation("Processing command: " + m.CommandName);
                return await Task.FromResult(new GenericCommandResponse()
                {
                    Status = 200,
                    ReponsePayload = JsonSerializer.Serialize(new { myResponse = "whatever" })
                });
            };

            client.genericDesiredUpdateProperty.OnProperty_Updated = async m =>
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
                await client.SendTelemetryAsync(new
                {
                    location = new
                    {
                        lat = -114.0298,
                        lon = 34.5574,
                        alt = 657.8799
                    },
                    temperature = 23
                },
                stoppingToken);
                _logger.LogInformation("sending telemetry");
                await Task.Delay(5000, stoppingToken);
            }
        }

        
    }
}
