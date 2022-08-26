﻿//  <auto-generated/> 


using Rido.Mqtt.Client;
using Rido.Mqtt.Client.TopicBindings;
using Rido.MqttCore;
using Rido.MqttCore.PnP;
using static Rido.MqttCore.Birth.BirthConvention;

namespace pnp_memmon_hive
{
    public class memmon : PnPClient, Imemmon
    {
        const string modelId = "dtmi:rido:pnp:memmon;1";
        //public string InitialState { get; set; }

        public IReadOnlyProperty<DateTime> Property_started { get; set; }
        public IWritableProperty<bool> Property_enabled { get; set; }
        public IWritableProperty<int> Property_interval { get; set; }
        public ITelemetry<double> Telemetry_workingSet { get; set; }
        public ICommand<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response> Command_getRuntimeStats { get; set; }

        private memmon(IMqttConnection c) : base(c)
        {
            Property_started = new ReadOnlyProperty<DateTime>(c, "started");
            Property_interval = new WritableProperty<int>(c, "interval");
            Property_enabled = new WritableProperty<bool>(c, "enabled");
            Telemetry_workingSet = new Telemetry<double>(c, "workingSet");
            Command_getRuntimeStats = new Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response>(c, "getRuntimeStats");
        }

        public static async Task<memmon> CreateClientAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            var cs = new ConnectionSettings(connectionString) { ModelId = modelId };
            var hub = await new Rido.Mqtt.MqttNet4Adapter.MqttNetClientConnectionFactory().CreateBasicClientAsync(cs);
            var client = new memmon(hub) ;
            //client.InitialState = await client.GetTwinAsync(cancellationToken);
            return client;
        }
    }



}
