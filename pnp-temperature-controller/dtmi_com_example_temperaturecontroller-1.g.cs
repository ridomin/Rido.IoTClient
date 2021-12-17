using dtmi_azure_devicemanagement;
using MQTTnet.Client;
using pnp_temperature_controller;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System.Text.Json;

namespace dtmi_com_example
{
    internal class TemperatureController : IoTHubPnPClient
    {
        const string modelId = "dtmi:com:example:TemperatureController;1";
        internal Telemetry<double> Telemetry_workingSet;
        internal ReadOnlyProperty<string> Property_serialNumber;
        internal Command<Cmd_reboot_Req, EmptyCommandResponse> Command_reboot;
        internal ThermostatComponent Component_thermostat1;
        internal ThermostatComponent Component_thermostat2;
        internal DeviceInformationComponent Component_deviceInfo;

        public TemperatureController(IMqttClient c) : base(c)
        {
            Telemetry_workingSet = new Telemetry<double>(c, "workingSet");
            Property_serialNumber = new ReadOnlyProperty<string>(c, "serialNumber");
            Command_reboot = new Command<Cmd_reboot_Req, EmptyCommandResponse>(c, "reboot");
            Component_thermostat1 = new ThermostatComponent(c, "thermostat1");
            Component_thermostat2 = new ThermostatComponent(c, "thermostat2");
            Component_deviceInfo = new DeviceInformationComponent(c, "deviceInformation");
        }

        public static async Task<TemperatureController> CreateAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            var cs = new ConnectionSettings(connectionString) { ModelId = modelId };
            var connection = await IoTHubConnectionFactory.CreateAsync(cs, cancellationToken);
            var client = new TemperatureController(connection) { ConnectionSettings = cs };
            client.InitialTwin = await client.GetTwinAsync();
            return client;
        }
    }

    class Cmd_reboot_Req : IBaseCommandRequest<Cmd_reboot_Req>
    {
        DateTime since;
        public Cmd_reboot_Req DeserializeBody(string payload)
        {
            return new Cmd_reboot_Req
            {
                since = JsonSerializer.Deserialize<DateTime>(payload)
            };
        }
    }
}
