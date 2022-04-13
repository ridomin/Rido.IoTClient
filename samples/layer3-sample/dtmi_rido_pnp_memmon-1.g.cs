
using Rido.MqttCore;
using Rido.PnP;
using Rido.PnP.TopicBindings;

namespace layer3_sample
{
    internal class dtmi_rido_pnp_memmon : PnPClient, Imemmon
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

        internal static async Task<dtmi_rido_pnp_memmon> CreateAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            var mqtt = await new Rido.Mqtt.MqttNet4Adapter.MqttNetClientConnectionFactory().CreateBasicClientAsync(cs, cancellationToken);
            var client = new dtmi_rido_pnp_memmon(mqtt);
            await client.Announce(new BirthMessage { ModelId = modelId });
            return client;
        }
    }
   
}
