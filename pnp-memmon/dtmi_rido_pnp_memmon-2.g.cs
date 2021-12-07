﻿//  <auto-generated/> 

using MQTTnet.Client;
using Rido.IoTClient;

using Rido.IoTClient.Hive;
using Rido.IoTClient.Hive.TopicBindings;

namespace dtmi_rido_pnp
{
    public class memmon_hive : HiveClient
    {
        const string modelId = "dtmi:rido:pnp:memmon;1";

        public ReadOnlyProperty<DateTime> Property_started;
        public WritableProperty<bool> Property_enabled;
        public WritableProperty<int> Property_interval;
        public TelemetryBinder<double> Telemetry_workingSet;
        public CommandBinder<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response> Command_getRuntimeStats;

        private memmon_hive(IMqttClient c) : base(c)
        {
            Property_started = new ReadOnlyProperty<DateTime>(c, "started");
            Property_interval = new WritableProperty<int>(c, "interval");
            Property_enabled = new WritableProperty<bool>(c, "enabled");
            Telemetry_workingSet = new TelemetryBinder<double>(c, c.Options.ClientId, "workingSet");
            Command_getRuntimeStats = new CommandBinder<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response>(c, "getRuntimeStats");
        }

        public static async Task<memmon_hive> CreateClientAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            var cs = new ConnectionSettings(connectionString)
            {
                ModelId = modelId
            };
            IMqttClient mqtt = await HiveClient.CreateAsync(cs, cancellationToken);
            var client = new memmon_hive(mqtt);
            client.ConnectionSettings = cs;
            //client.InitialTwin = await client.GetTwinAsync(cancellationToken);
            return client;
        }
    }
}
