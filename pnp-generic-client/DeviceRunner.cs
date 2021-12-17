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
            var client = await GenericPnPClient.CreateAsync(new ConnectionSettings(_configuration.GetConnectionString("cs")), stoppingToken);
            await client.ReportPropertyAsync(new { started = DateTime.Now }, stoppingToken);
            var twin = await client.GetTwinAsync(stoppingToken);
            _logger.LogInformation(twin);

            _logger.LogInformation("connected " + client.ConnectionSettings);

            client.Command.OnCmdDelegate = async m =>
            {
                _logger.LogInformation("Processing command: " + m.CommandName);
                return await Task.FromResult(new GenericCommandResponse()
                {
                    Status = 200,
                    ReponsePayload = JsonSerializer.Serialize(new { myResponse = "whatever" })
                });
            };

            client.genericDesiredUpdatePropertyBinder.OnProperty_Updated = async m =>
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
                await client.SendTelemetryAsync(new { temp = 32.3 }, stoppingToken);
                _logger.LogInformation("sending telemetry");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
