# Rido.IoTClient

This project implements a MQTT device client for Azure IoT Hub, providing three layers of abstraction:

- Layer 1. Connection enabling BYO MQTT library
- Layer 2. Implements Azure IoT Hub primitives
- Layer 3. Provides a typed API to implement [IoT Plug and Play devices](https://docs.microsoft.com/en-us/azure/iot-develop/overview-iot-plug-and-play)

## Quickstart

See this [sample](samples/layer2-sample) for a complete project

```cs
using Rido.Mqtt.HubClient;

var client = await HubMqttClient.CreateFromConnectionStringAsync(_configuration.GetConnectionString("myConnectinStringKey"));

// Send Telemetry
var puback = await client.SendTelemetryAsync(new { workingSet = Environment.WorkingSet });

// Report properties
var v = await client.ReportPropertyAsync(new { started = DateTime.Now });
_logger.LogInformation($"Property updated with version {v}");

// Retrieve Twin
var twin = await client.GetTwinAsync();
_logger.LogInformation(twin);

// Implement a command
client.OnCommandReceived = async m =>
{
    _logger.LogInformation("Processing command: " + m.CommandName);
    return await Task.FromResult(new CommandResponse()
    {
        Status = 200,
        ReponsePayload = JsonSerializer.Serialize(new { myResponse = "whatever" })
    });
};

// Implement desired property updates
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
```

## Connection Settings

This library implements a compatible *connection string* with Azure IoT SDK Device Client, and adds some new properties:

- `HostName` Azure IoT Hub hostname (FQDN)
- `IdScope` DPS IdScope 
- `DeviceId` Device Identity 
- `SharedAccessKey` Device Shared Access Key
- `X509Key` __pathtopfx>|<pfxpassword__
- `ModelId` DTDL Model ID in DTMI format to create PnP Devices
- `ModuleId` Device Module Identity
- `Auth` Device Authentication: [SAS, X509]
- `SasMinutes` SasToken expire time in minutes, default to `60`.

Sample Connection String

```cs
$"HostName=test.azure-devices.net;DeviceId=myDevice;ModuleId=myModule;SharedAccessKey=<moduleSasKey>;ModelId=dtmi:my:model;1";SasMinutes=120
```

> Note: All samples use the connection settings in the `ConnectionString` configuration, available in the `appSettings.json` file, or as the environment variable `ConnectionString__Key`.

### DPS Client

The Connection Settings allow to use DPS (useful to connect to IoT Central) by adding the `IdScope`, when found it will provision the device using `Rido.Mqtt.DpsClient`.

## Advanced Samples

### BYO MQTT

This library allows to work with any MQTT Library by implementing the `IMqttBaseClient` interface defined in `Rido.MqttCore` assembly.

There are 3 adapters showing how to implement the interface for `MQTTNet3`, `MQTTNet4-preview` and `M2MMqttDotnetCore`, all in the [mqtt-adapters](mqtt-adapters) folder.

`HubMqttClient` can be initialized with any of these adapters by using the constructor:

```cs
//IMqttBaseClient adapter = await new MqttNet3Adapter.MqttNetClientConnectionFactory().CreateHubClientAsync(_configuration.GetConnectionString("cs"), stoppingToken);
//IMqttBaseClient adapter = await new MqttNet4Adapter.MqttNetClientConnectionFactory().CreateHubClientAsync(_configuration.GetConnectionString("cs"),stoppingToken);
IMqttBaseClient adapter = await new M2MAdapter.M2MClientConnectionFactory().CreateHubClientAsync(_configuration.GetConnectionString("cs"), stoppingToken);
var client = new HubMqttClient(adapter);
```

> Note: By default the MQTTNet3 adapter is using in the `CreateFromConnectionStringAsync` method.

### IoT Plug and Play

The library also allows a high-level API to implement IoT Plug and Play concepts:

- Telemetry<T>
- ReadOnlyProperty<T>
- WritableProperty<T>
- Command<Request, Response>

Full sample available in [layer3-sample](samples/layer3-sample)

Given a DTDL interface like

```json
{
  "@context": "dtmi:dtdl:context;2",
  "@id": "dtmi:rido:pnp:memmon;1",
  "@type": "Interface",
  "contents": [
    {
      "@type": "Property",
      "name": "started",
      "schema": "dateTime"
    },
    {
      "@type": "Property",
      "name": "enabled",
      "schema": "boolean",
      "writable": true
    },
    {
      "@type": [ "Property", "TimeSpan" ],
      "name": "interval",
      "schema": "integer",
      "writable": true,
      "unit": "second"
    },
    {
      "@type": [ "Telemetry", "DataSize" ],
      "name": "workingSet",
      "schema": "double",
      "unit": "byte"
    },
    {
      "@type": "Command",
      "name": "getRuntimeStats",
```
> Note: full interface available [here](samples\layer3-sample\dtmi_rido_pnp_memmon-1.json)

We can generate a supporting class like

```cs
internal class dtmi_rido_pnp_memmon : HubMqttClient, Imemmon
{
    const string modelId = "dtmi:rido:pnp:memmon;1";

    public IReadOnlyProperty<DateTime> Property_started { get; set; }
    public IWritableProperty<bool> Property_enabled { get; set; }
    public IWritableProperty<int> Property_interval { get; set; }
    public ITelemetry<double> Telemetry_workingSet { get; set; }
    public ICommand<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response> Command_getRuntimeStats { get; set; }

    private dtmi_rido_pnp_memmon(IHubMqttClient c) : base(c.Connection)
    {
        Property_started = new ReadOnlyProperty<DateTime>(c.Connection, "started");
        Property_interval = new WritableProperty<int>(c.Connection, "interval");
        Property_enabled = new WritableProperty<bool>(c.Connection, "enabled");
        Telemetry_workingSet = new Telemetry<double>(c.Connection, "workingSet");
        Command_getRuntimeStats = new Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response>(c.Connection, "getRuntimeStats");
    }

    internal static async Task<dtmi_rido_pnp_memmon> CreateAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        var mqtt = await HubMqttClient.CreateFromConnectionStringAsync(connectionString + ";ModelId=" + modelId, cancellationToken);
        var client = new dtmi_rido_pnp_memmon(mqtt);
        return client;
    }
}
```

Offering a typed interface to implement the device logic

```cs
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    var client = await dtmi_rido_pnp_memmon.CreateAsync(_configuration.GetConnectionString("dps"), stoppingToken);

    var twin = await client.GetTwinAsync(stoppingToken);
    _logger.LogInformation(twin);

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

            
    await client.Property_interval.InitPropertyAsync(twin, 2, stoppingToken);
    await client.Property_enabled.InitPropertyAsync(twin, true, stoppingToken);

    client.Property_started.PropertyValue = DateTime.Now;
    await client.Property_started.ReportPropertyAsync(stoppingToken);


    while (!stoppingToken.IsCancellationRequested)
    {
        await client.Telemetry_workingSet.SendTelemetryAsync(Environment.WorkingSet, stoppingToken);
        await Task.Delay(client.Property_interval.PropertyValue.Value * 1000, stoppingToken);
    }
}
```

