using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System.Text.Json;

namespace pnp_temperature_controller
{
    class ThermostatComponent : Component<Thermostat>
    {
        public ThermostatComponent(IMqttClient c, string name) : base(c, name)
        {
            
            ComponentValue.Telemetry_temperature = new Telemetry<double>(c, "temperature", name);
            ComponentValue.Property_targetTemperature = new WritableProperty<double>(c, "targetTemperature", name);
            ComponentValue.Property_maxTempSinceLastReboot = new ReadOnlyProperty<double>(c, "maxTempSinceLastReboot", name);
            ComponentValue.Command_getMaxMinReport = new Command<Cmd_getMaxMinReport_Request, Cmd_getMaxMinReport_Response>(c, "getMaxMinReport", name);
        }
    }

    internal class Thermostat : ITwinSerializable
    {
        internal Telemetry<double> Telemetry_temperature;
        internal WritableProperty<double> Property_targetTemperature;
        internal ReadOnlyProperty<double> Property_maxTempSinceLastReboot;
        internal Command<Cmd_getMaxMinReport_Request, Cmd_getMaxMinReport_Response> Command_getMaxMinReport;

        public Dictionary<string, object> ToJsonDict()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("maxTempSinceLastReboot", Property_maxTempSinceLastReboot.PropertyValue);
            return dict;
        }
    }

    public class Cmd_getMaxMinReport_Request : IBaseCommandRequest<Cmd_getMaxMinReport_Request>
    {
        public DateTime since { get; set; }

        public Cmd_getMaxMinReport_Request DeserializeBody(string payload)
        {
            return new Cmd_getMaxMinReport_Request()
            {
                since = JsonSerializer.Deserialize<DateTime>(payload)
            };
        }

    }

    public class Cmd_getMaxMinReport_Response : BaseCommandResponse
    {
        public double maxTemp { get; set; }
        public double minTemp { get; set; }
        public double avgTemp { get; set; }
        public DateTimeOffset startTime { get; set; }
    }
}
