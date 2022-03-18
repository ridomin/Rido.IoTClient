using Rido.Mqtt.HubClient;
using Rido.Mqtt.HubClient.TopicBindings;
using Rido.Mqtt.MqttNet4Adapter;
using Rido.MqttCore;

namespace pnp_device_sample
{
    internal class dtmi_rido_pnp_memmon : HubMqttClient, Imemmon
    {
        const string modelId = "dtmi:rido:pnp:memmon;1";

        public IReadOnlyProperty<DateTime> Property_started { get; set; }
        public IWritableProperty<bool> Property_enabled { get; set; }
        public IWritableProperty<int> Property_interval { get; set; }
        public ITelemetry<double> Telemetry_workingSet { get; set; }
        public ICommand<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response> Command_getRuntimeStats { get; set; }

        private dtmi_rido_pnp_memmon(IMqttBaseClient c) : base(c)
        {
            Property_started = new ReadOnlyProperty<DateTime>(c, "started");
            Property_interval = new WritableProperty<int>(c, "interval");
            Property_enabled = new WritableProperty<bool>(c, "enabled");
            Telemetry_workingSet = new Telemetry<double>(c, "workingSet");
            Command_getRuntimeStats = new Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response>(c, "getRuntimeStats");
        }

        internal static async Task<dtmi_rido_pnp_memmon> CreateAsync(string connectionString, CancellationToken cancellationToken = default)
        {

            var mqtt = await new MqttNetClientConnectionFactory().CreateHubClientAsync(connectionString, cancellationToken);
            var client = new dtmi_rido_pnp_memmon(mqtt);
            return client;
        }
    }
   
}
