using dtmi_azure_devicemanagement;
using dtmi_com_example;
using Rido.IoTClient;
using System.Runtime.InteropServices;

namespace pnp_temperature_controller
{
    public class DeviceRunner : BackgroundService
    {
        private readonly ILogger<DeviceRunner> _logger;
        private readonly IConfiguration _configuration;


        static readonly Random random = new();
        static double RndDouble(double scaleFactor = 1.1) => random.NextDouble() * scaleFactor;
        double maxTemp1 = 0d;
        double maxTemp2 = 0d;
        readonly FixedSizeDictonary<DateTimeOffset, double> readings1 = new(1000) { { DateTimeOffset.Now, Math.Round(RndDouble(18), 1) } };
        readonly FixedSizeDictonary<DateTimeOffset, double> readings2 = new(1000) { { DateTimeOffset.Now, Math.Round(RndDouble(18), 1) } };
        double temperature1 = Math.Round(RndDouble(18), 1);
        double temperature2 = Math.Round(RndDouble(21), 1);


        TemperatureController client;
        public DeviceRunner(ILogger<DeviceRunner> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            client = await TemperatureController.CreateAsync(_configuration.GetConnectionString("cs"), stoppingToken);
            _logger.LogInformation("Connected" + client.ConnectionSettings);

            client.Command_reboot.OnCmdDelegate = Cmd_reboot_Handler;            
            client.Component_thermostat1.Property_targetTemperature.OnProperty_Updated = OnProperty_t1_targetTemperatue_Handler;
            client.Component_thermostat1.Command_getMaxMinReport.OnCmdDelegate = Cmd_t1_getMaxMinReport_Handler;
            
            client.Component_thermostat2.Property_targetTemperature.OnProperty_Updated = OnProperty_t2_targetTemperatue_Handler;
            client.Component_thermostat2.Command_getMaxMinReport.OnCmdDelegate = Cmd_t2_getMaxMinReport_Handler;

            await client.Component_thermostat1.Property_targetTemperature.InitPropertyAsync(client.InitialState, 22, stoppingToken);
            await client.Component_thermostat2.Property_targetTemperature.InitPropertyAsync(client.InitialState, 25, stoppingToken);
            
            ThisDeviceInfo(client.Component_deviceInfo);
            await client.Component_deviceInfo.ReportPropertyAsync(stoppingToken);

            Console.WriteLine(client.Component_thermostat1.Property_targetTemperature.PropertyValue.Value.ToString());
            Console.WriteLine(client.Component_thermostat2.Property_targetTemperature.PropertyValue.Value.ToString());
                
            while (!stoppingToken.IsCancellationRequested)
            {
                temperature1 = Math.Round((temperature1 % 2) == 0 ? temperature1 + RndDouble(0.3) : temperature1 - RndDouble(0.2), 2);
                readings1.Add(DateTimeOffset.Now, temperature1);

                temperature2 = Math.Round((temperature1 % 2) == 0 ? temperature1 + RndDouble(0.3) : temperature2 - RndDouble(0.2), 2);
                readings1.Add(DateTimeOffset.Now, temperature2);

                await client.Telemetry_workingSet.SendTelemetryAsync(Environment.WorkingSet, stoppingToken);
                await client.Component_thermostat1.Telemetry_temperature.SendTelemetryAsync(temperature1, stoppingToken);
                await client.Component_thermostat2.Telemetry_temperature.SendTelemetryAsync(temperature2, stoppingToken);
                Console.WriteLine($"-> t: t1 {temperature1} , t2 {temperature2}");
                await Task.Delay(15000, stoppingToken);
            }
        }

        async Task<PropertyAck<double>> OnProperty_t1_targetTemperatue_Handler(PropertyAck<double> prop)
        {
            Console.WriteLine("\n<- w: t1-targetTemperature received " + prop.Value);
            client.Component_thermostat1.Property_targetTemperature.PropertyValue.Status = 202;
            var v = await client.Component_thermostat1.Property_targetTemperature.ReportPropertyAsync();
            Console.WriteLine("t1 in progress updated to v: " + v);
            await AdjustTempInStepsAsync_1(prop);
            return await Task.FromResult(prop);
        }

        async Task<PropertyAck<double>> OnProperty_t2_targetTemperatue_Handler(PropertyAck<double> prop)
        {
            Console.WriteLine("\n<- w: t2-targetTemperature received " + prop.Value);
            client.Component_thermostat2.Property_targetTemperature.PropertyValue.Status = 202;
            var v = await client.Component_thermostat2.Property_targetTemperature.ReportPropertyAsync();
            Console.WriteLine("t2 in progress updated to v: " + v);
            await AdjustTempInStepsAsync_2(prop);
            return await Task.FromResult(prop);
        }


        private async Task<EmptyCommandResponse> Cmd_reboot_Handler(Cmd_reboot_Req req)
        {
            _logger.LogInformation("Processing reboot: " + req.delay);
            return await Task.FromResult(new EmptyCommandResponse() { Status = 200 });
        }

        async Task AdjustTempInStepsAsync_1(PropertyAck<double> prop)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(prop);
            Console.WriteLine("\n adjusting t1 temp  to: " + prop.Value);

            double step = (prop.Value - temperature1) / 5d;
            for (int i = 1; i <= 5; i++)
            {
                temperature1 = Math.Round(temperature1 + step, 1);
                Console.WriteLine($"\r-> t: t1 - temperature {temperature1} \t");
                readings1.Add(DateTimeOffset.Now, temperature1);
                await Task.Delay(500);
            }
            prop.Value = temperature1;
            prop.Status = 200;
            prop.Description = "Temp updated to " + temperature1;
            prop.Version = prop.DesiredVersion;
            client.Component_thermostat1.Property_targetTemperature.PropertyValue = prop;
            await client.Component_thermostat1.Property_targetTemperature.ReportPropertyAsync();

            Console.WriteLine("\n t1 temp adjusted to: " + prop.Value);
        }

        async Task AdjustTempInStepsAsync_2(PropertyAck<double> prop)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(prop);
            
            Console.WriteLine("\n adjusting t2 temp  to: " + prop.Value);

            double step = (prop.Value - temperature2) / 5d;
            for (int i = 1; i <= 5; i++)
            {
                temperature2 = Math.Round(temperature2 + step, 1);

                Console.WriteLine($"\r-> t: t2 - temperature {temperature2} \t");
                readings2.Add(DateTimeOffset.Now, temperature2);
                await Task.Delay(500);
            }
            prop.Value = temperature2;
            prop.Status = 200;
            prop.Description = "Temp updated to " + temperature2;
            prop.Version = prop.DesiredVersion;
            client.Component_thermostat2.Property_targetTemperature.PropertyValue = prop;
            await client.Component_thermostat2.Property_targetTemperature.ReportPropertyAsync();

            Console.WriteLine("\n t2 temp adjusted to: " + prop.Value);
        }


        async Task<Cmd_getMaxMinReport_Response> Cmd_t1_getMaxMinReport_Handler(Cmd_getMaxMinReport_Request req)
        {
            ArgumentNullException.ThrowIfNull(client);
            Console.WriteLine("\n<- c: t1-getMaxMinReport " + req.since);

            if (readings1.Values.Max<double>() > maxTemp1)
            {
                maxTemp1 = readings1.Values.Max<double>();
                _ = client.Component_thermostat1.Property_maxTempSinceLastReboot.ReportPropertyAsync(maxTemp1);

                Console.WriteLine($"\n-> r: maxTempSinceLastReboot {maxTemp1}");
            }

            await Task.Delay(100);
            Dictionary<DateTimeOffset, double> filteredReadings = readings1
                                           .Where(i => i.Key > req.since)
                                           .ToDictionary(i => i.Key, i => i.Value);
            return new Cmd_getMaxMinReport_Response
            {
                maxTemp = filteredReadings.Values.Max<double>(),
                minTemp = filteredReadings.Values.Min<double>(),
                avgTemp = filteredReadings.Values.Average(),
                startTime = filteredReadings.Keys.Min(),
                Status = 200
            };
        }

        async Task<Cmd_getMaxMinReport_Response> Cmd_t2_getMaxMinReport_Handler(Cmd_getMaxMinReport_Request req)
        {
            ArgumentNullException.ThrowIfNull(client);
            Console.WriteLine("\n<- c: t2-getMaxMinReport " + req.since);

            if (readings2.Values.Max<double>() > maxTemp2)
            {
                maxTemp2 = readings2.Values.Max<double>();
                _ = client.Component_thermostat2.Property_maxTempSinceLastReboot.ReportPropertyAsync(maxTemp2);

                Console.WriteLine($"\n-> r: maxTempSinceLastReboot {maxTemp2}");
            }

            await Task.Delay(100);
            Dictionary<DateTimeOffset, double> filteredReadings = readings2
                                           .Where(i => i.Key > req.since)
                                           .ToDictionary(i => i.Key, i => i.Value);
            return new Cmd_getMaxMinReport_Response
            {
                maxTemp = filteredReadings.Values.Max<double>(),
                minTemp = filteredReadings.Values.Min<double>(),
                avgTemp = filteredReadings.Values.Average(),
                startTime = filteredReadings.Keys.Min(),
                Status = 200
            };
        }

       

        static void ThisDeviceInfo(DeviceInformation di)
        {
            di.Property_manufacturer.PropertyValue = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
            di.Property_model.PropertyValue = Environment.OSVersion.Platform.ToString();
            di.Property_swVersion.PropertyValue = Environment.OSVersion.VersionString;
            di.Property_osName.PropertyValue = Environment.GetEnvironmentVariable("OS");
            di.Property_processorArchitecture.PropertyValue = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            di.Property_processorManufacturer.PropertyValue = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
            di.Property_totalStorage.PropertyValue = System.IO.DriveInfo.GetDrives()[0].TotalSize;
            di.Property_totalMemory.PropertyValue = System.Environment.WorkingSet;
        }
    }
}