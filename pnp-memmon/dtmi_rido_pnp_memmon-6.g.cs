﻿//  <auto-generated/> 

using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.Aws;
using Rido.IoTClient.PnPMqtt.TopicBindings;

namespace dtmi_rido_pnp_AwsBroker
{
    public class memmon : BaseClient
    {
        const string modelId = "dtmi:rido:pnp:memmon;1";
        
        public ReadOnlyProperty<DateTime> Property_started;
        public WritableProperty<bool> Property_enabled;
        public WritableProperty<int> Property_interval;
        public Telemetry<double> Telemetry_workingSet;
        public Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response> Command_getRuntimeStats;

        private memmon(IMqttClient c) : base(c)
        {
            Property_started = new ReadOnlyProperty<DateTime>(c, "started");
            Property_interval = new WritableProperty<int>(c, "interval");
            Property_enabled = new WritableProperty<bool>(c, "enabled");
            Telemetry_workingSet = new Telemetry<double>(c, "workingSet");
            Command_getRuntimeStats = new Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response>(c, "getRuntimeStats");
        }

        public static async Task<memmon> CreateClientAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            var cs = new ConnectionSettings(connectionString){ModelId = modelId};
            var connection = await AwsConnectionFactory.CreateAsync(cs, cancellationToken);
            var client = new memmon(connection){ConnectionSettings = cs};
            return client;
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
                DiagnosticsMode = System.Text.Json.JsonSerializer.Deserialize<DiagnosticsMode>(payload)
            };
        }
    }

    public class Cmd_getRuntimeStats_Response : BaseCommandResponse
    {
        public Dictionary<string, string> diagnosticResults { get; set; } = new Dictionary<string, string>();
    }
}
