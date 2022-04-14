﻿//  <auto-generated/> 

using Rido.Mqtt.HubClient;
using Rido.MqttCore.PnP;


namespace layer3_sample
{
    public interface Imemmon 
    {
        public IReadOnlyProperty<DateTime> Property_started { get; set; }
        public IWritableProperty<bool> Property_enabled { get; set; }
        public IWritableProperty<int> Property_interval { get; set; }
        public ITelemetry<double> Telemetry_workingSet { get; set; }
        public ICommand<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response> Command_getRuntimeStats { get; set; }

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
                DiagnosticsMode = System.Text.Json.JsonSerializer.Deserialize<DiagnosticsMode>(payload)
            };
        }
    }

    public class Cmd_getRuntimeStats_Response : BaseCommandResponse
    {
        public Dictionary<string, string> diagnosticResults { get; set; } = new Dictionary<string, string>();
    }
}
