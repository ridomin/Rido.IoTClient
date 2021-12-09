using dtmi_rido_pnp_sample;
using Humanizer;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System.Diagnostics;
using System.Text;

namespace pnp_memmon_component;

public class DeviceRunner : BackgroundService
{
    private readonly ILogger<DeviceRunner> _logger;
    private readonly IConfiguration _configuration;
    readonly Stopwatch clock = Stopwatch.StartNew();

    double telemetryWorkingSet = 0;

    int telemetryCounter = 0;
    int commandCounter = 0;
    int twinRecCounter = 0;
    int reconnectCounter = 0;

    dtmi_rido_pnp_sample.memmon client;

    const bool default_enabled = true;
    const int default_interval = 8;

    public DeviceRunner(ILogger<DeviceRunner> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Connecting..");
        client = await dtmi_rido_pnp_sample.memmon.CreateDeviceClientAsync(_configuration.GetConnectionString("hub"), stoppingToken);
        _logger.LogInformation("Connected");

        client.Connection.DisconnectedAsync += async e => await Task.FromResult(reconnectCounter++);

        client.Property_memMon_enabled.OnProperty_Updated = Property_memMon_enabled_UpdateHandler;
        client.Property_memMon_interval.OnProperty_Updated = Property_memMon_interval_UpdateHandler;
        client.Command_getRuntimeStats_Binder.OnCmdDelegate = Command_memMon_getRuntimeStats_Handler;

        await client.Property_memMon_enabled.InitPropertyAsync(client.InitialTwin, default_enabled, stoppingToken);
        await client.Property_memMon_interval.InitPropertyAsync(client.InitialTwin, default_interval, stoppingToken);
        await client.Property_memMon_started.UpdateTwinPropertyAsync(DateTime.Now, stoppingToken);
        await client.Component_deviceInfo.UpdateTwinAsync(ThisDeviceInfo);

        RefreshScreen(this);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (client?.Property_memMon_enabled?.PropertyValue.Value == true)
            {
                telemetryWorkingSet = Environment.WorkingSet;
                await client.Telemetry_memMon_workingSet.SendTelemetryAsync(telemetryWorkingSet, stoppingToken);
                telemetryCounter++;
            }
            await Task.Delay(client.Property_memMon_interval.PropertyValue.Value * 1000, stoppingToken);
        }
    }

    async Task<PropertyAck<bool>> Property_memMon_enabled_UpdateHandler(PropertyAck<bool> req)
    {
        ArgumentNullException.ThrowIfNull(client);
        twinRecCounter++;
        var ack = new PropertyAck<bool>("enabled", "memMon")
        {
            Description = "desired notification accepted",
            Status = 200,
            Version = req.Version,
            Value = req.Value
        };
        client.Property_memMon_enabled.PropertyValue = ack;
        return await Task.FromResult(ack);
    }

    async Task<PropertyAck<int>> Property_memMon_interval_UpdateHandler(PropertyAck<int> req)
    {
        ArgumentNullException.ThrowIfNull(client);
        twinRecCounter++;
        var ack = new PropertyAck<int>("interval", "memMon")
        {
            Description = (client.Property_memMon_enabled?.PropertyValue.Value == true) ? "desired notification accepted" : "disabled, not accepted",
            Status = (client.Property_memMon_enabled?.PropertyValue.Value == true) ? 200 : 205,
            Version = req.Version,
            Value = req.Value
        };
        client.Property_memMon_interval.PropertyValue = ack;
        return await Task.FromResult(ack);
    }

    async Task<Cmd_getRuntimeStats_Response> Command_memMon_getRuntimeStats_Handler(Cmd_getRuntimeStats_Request req)
    {
        commandCounter++;
        var result = new Cmd_getRuntimeStats_Response()
        {
            Status = 200
        };

        result.diagnosticResults.Add("machine name", Environment.MachineName);
        result.diagnosticResults.Add("os version", Environment.OSVersion.ToString());
        if (req.DiagnosticsMode == DiagnosticsMode.complete)
        {
            result.diagnosticResults.Add("this app:", System.Reflection.Assembly.GetExecutingAssembly()?.FullName ?? "");
        }
        if (req.DiagnosticsMode == DiagnosticsMode.full)
        {
            result.diagnosticResults.Add($"twin receive: ", twinRecCounter.ToString());
            result.diagnosticResults.Add($"twin sends: ", RidCounter.Current.ToString());
            result.diagnosticResults.Add("telemetry: ", telemetryCounter.ToString());
            result.diagnosticResults.Add("command: ", commandCounter.ToString());
            result.diagnosticResults.Add("reconnects: ", reconnectCounter.ToString());
        }
        return await Task.FromResult(result);
    }

    private void RefreshScreen(object state)
    {
        string RenderData()
        {
            void AppendLineWithPadRight(StringBuilder sb, string s) => sb.AppendLine(s?.PadRight(Console.BufferWidth - 1));

            string enabled_value = client?.Property_memMon_enabled?.PropertyValue.Value.ToString();
            string interval_value = client?.Property_memMon_interval?.PropertyValue.Value.ToString();
            StringBuilder sb = new();
            AppendLineWithPadRight(sb, " ");
            AppendLineWithPadRight(sb, client?.ConnectionSettings?.HostName);
            AppendLineWithPadRight(sb, $"{client?.ConnectionSettings?.DeviceId} ({client?.ConnectionSettings?.Auth})");
            AppendLineWithPadRight(sb, " ");
            AppendLineWithPadRight(sb, String.Format("{0:9} | {1:8} | {2:15} | {3}", "Component", "Property", "Value".PadRight(15), "Version"));
            AppendLineWithPadRight(sb, String.Format("{0:9} | {1:8} | {2:15} | {3}", "---------", "--------", "-----".PadRight(15, '-'), "------"));
            AppendLineWithPadRight(sb, String.Format("{0:9} | {1:8} | {2:15} | {3}", "memMon".PadRight(9), "enabled".PadRight(8), enabled_value?.PadLeft(15), client?.Property_memMon_enabled?.PropertyValue.Version));
            AppendLineWithPadRight(sb, String.Format("{0:9} | {1:8} | {2:15} | {3}", "memMon".PadRight(9), "interval".PadRight(8), interval_value?.PadLeft(15), client?.Property_memMon_interval?.PropertyValue.Version));
            AppendLineWithPadRight(sb, String.Format("{0:9} | {1:8} | {2:15} | {3}", "memMon".PadRight(9), "started".PadRight(8), client?.Property_memMon_started.PropertyValue.ToShortTimeString().PadLeft(15), client?.Property_memMon_started?.Version));
            AppendLineWithPadRight(sb, " ");
            AppendLineWithPadRight(sb, $"Reconnects: {reconnectCounter}");
            AppendLineWithPadRight(sb, $"Telemetry: {telemetryCounter}");
            AppendLineWithPadRight(sb, $"Twin receive: {twinRecCounter}");
            AppendLineWithPadRight(sb, $"Twin send: {RidCounter.Current}");
            AppendLineWithPadRight(sb, $"Command messages: {commandCounter}");
            AppendLineWithPadRight(sb, " ");
            AppendLineWithPadRight(sb, $"memMon.workingSet: {telemetryWorkingSet.Bytes()}");
            AppendLineWithPadRight(sb, " ");
            AppendLineWithPadRight(sb, $"Time Running: {TimeSpan.FromMilliseconds(clock.ElapsedMilliseconds).Humanize(3)}");
            AppendLineWithPadRight(sb, " ");
            AppendLineWithPadRight(sb, " ");
            return sb.ToString();
        }

        Console.SetCursorPosition(0, 0);
        Console.WriteLine(RenderData());
        var screenRefresher = new Timer(RefreshScreen, this, 1000, 0);
    }

    static dtmi_azure_devicemanagement.DeviceInformation ThisDeviceInfo
    {
        get => new()
        {
            manufacturer = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"),
            model = Environment.OSVersion.Platform.ToString(),
            softwareVersion = Environment.OSVersion.VersionString,
            operatingSystemName = Environment.GetEnvironmentVariable("OS"),
            processorArchitecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"),
            processorManufacturer = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"),
            totalStorage = System.IO.DriveInfo.GetDrives()[0].TotalSize,
            totalMemory = System.Environment.WorkingSet
        };
    }   
}
