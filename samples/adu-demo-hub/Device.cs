using dtmi_rido_pnp;
using Rido.MqttCore.PnP;

namespace adu_demo_hub
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

        memmon? client;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            client = await memmon.CreateClientAsync(_configuration.GetConnectionString("hub"), stoppingToken);
            _logger.LogInformation($"Connected: {client.Connection.ConnectionSettings}");

            client.Component_deviceUpdate.Property_service.OnProperty_Updated = Property_deviceUpdate_service_UpdateHandler;

            client.Component_deviceInformation.Property_swVersion.PropertyValue = "0.0.1";
            await client.Component_deviceInformation.ReportPropertyAsync(stoppingToken);

            await client.Component_deviceUpdate.Property_service.InitPropertyAsync(client.InitialState, null, stoppingToken);


            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(60000, stoppingToken);
            }
        }

        async Task<PropertyAck<serviceMetadata>> Property_deviceUpdate_service_UpdateHandler(PropertyAck<serviceMetadata> req)
        {
            ArgumentNullException.ThrowIfNull(client);

            _logger.LogInformation($"Received Update: {req.Value.workflow.action } {req.Value.updateManifest}");

            Console.WriteLine("");
            Console.WriteLine("Accept Update (Y/n)?");
            var userResponse = Console.ReadLine();
            Console.WriteLine("");

            PropertyAck<serviceMetadata> ack = new PropertyAck<serviceMetadata>(client.Component_deviceUpdate.Property_service.PropertyName);

            if (userResponse != null && userResponse == "Y")
            {
                serviceMetadata updateResponse =  await PerformUpdate(req.Value);

                ack.Description = "service update received";
                ack.Status = 200;
                ack.Version = req.Version;
                ack.Value = updateResponse;
            }
            else
            {
                ack.Description = "update postponed";
                ack.Status = 203;
                ack.Version = req.Version;
            }
            Console.WriteLine(ack.Description);
            return ack;
        }

        private async Task<serviceMetadata> PerformUpdate(serviceMetadata data)
        {
            foreach (var file in data.fileUrls)
            {
                Console.WriteLine($"Downloading {file.Key} {file.Value}");
                byte[] bytes  = await new HttpClient().GetByteArrayAsync(file.Value);
                Console.WriteLine($"Downloaded {bytes.Length} bytes");
            }
            return new serviceMetadata
            {
                workflow = new workflowMetadata
                {
                    id = data.workflow.id,
                    action = 0
                }
            };
        }
    }
}