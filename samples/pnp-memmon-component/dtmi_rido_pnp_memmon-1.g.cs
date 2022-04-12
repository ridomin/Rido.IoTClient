using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub.TopicBindings;
//using Rido.IoTClient.Hive.TopicBindings;
using System.Text.Json;

namespace dtmi_rido_pnp
{
    public class memmon : Component
    {
        public ReadOnlyProperty<DateTime> Property_started;
        public WritableProperty<bool> Property_enabled;
        public WritableProperty<int> Property_interval;
        public Telemetry<double> Telemetry_workingSet;
        public Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response> Command_getRuntimeStats;

        public memmon(IMqttClient c, string name) : base(c, name)
        {
            Property_interval = new WritableProperty<int>(c, "interval", name);
            Property_enabled = new WritableProperty<bool>(c, "enabled", name);
            Property_started = new ReadOnlyProperty<DateTime>(c, "started", name);
            Telemetry_workingSet = new Telemetry<double>(c, "workingSet", name);
            Command_getRuntimeStats = new Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response>(c, "getRuntimeStats", name);
        }

        public override  Dictionary<string, object> ToJsonDict()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("started", Property_started.PropertyValue);
            return dict;
        }
    }

    public enum DiagnosticsMode
    {
        minimal = 0,
        complete = 1,
        full = 2
    }

    public class Cmd_getRuntimeStats_Request : IBaseCommandRequest<Cmd_getRuntimeStats_Request>
    {
        public DiagnosticsMode DiagnosticsMode { get; set; }

        public Cmd_getRuntimeStats_Request DeserializeBody(string payload)
        {
            return new Cmd_getRuntimeStats_Request()
            {
                DiagnosticsMode = JsonSerializer.Deserialize<DiagnosticsMode>(payload)
            };
        }
    }

    public class Cmd_getRuntimeStats_Response : BaseCommandResponse
    {
        public Dictionary<string, string> diagnosticResults { get; set; } = new Dictionary<string, string>();
    }
}
