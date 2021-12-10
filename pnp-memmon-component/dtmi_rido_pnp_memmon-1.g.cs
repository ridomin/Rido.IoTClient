using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System.Text.Json;

namespace dtmi_rido_pnp
{

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

    public class memMonComponent : Component<memmon>
    {
        public memMonComponent(IMqttClient c, string name) : base(c, name)
        {
            ComponentValue = new memmon();
            ComponentValue.Property_interval = new WritableProperty<int>(c, "interval", name);
            ComponentValue.Property_enabled = new WritableProperty<bool>(c, "enabled", name);
            ComponentValue.Property_started = new ReadOnlyProperty<DateTime>(c, "started", name);
            ComponentValue.Telemetry_workingSet = new Telemetry<double>(c, "workingSet", name);
            ComponentValue.Command_getRuntimeStats = new Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response>(c, "getRuntimeStats", name);
        }
    }

    public class memmon : ITwinSerializable
    {
        public ReadOnlyProperty<DateTime> Property_started;
        public WritableProperty<bool> Property_enabled;
        public WritableProperty<int> Property_interval;
        public Telemetry<double> Telemetry_workingSet;
        public Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response> Command_getRuntimeStats;

        public Dictionary<string, object> ToJsonDict()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("started", Property_started.PropertyValue);
            return dict;
        }
    }
}
