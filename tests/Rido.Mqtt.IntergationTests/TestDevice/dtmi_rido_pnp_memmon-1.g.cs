﻿//  <auto-generated/> 

using pnp_memmon;
using Rido.Mqtt.HubClient;
using Rido.Mqtt.HubClient.TopicBindings;
using Rido.MqttCore;
using Rido.MqttCore.PnP;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace dtmi_rido_pnp_IoTHubClassic
{
    public class memmon : HubMqttClient, Imemmon
    {
        const string modelId = "dtmi:rido:pnp:memmon;1";
        public string InitialState { get; set; }

        public IReadOnlyProperty<DateTime> Property_started { get; set; }
        public IWritableProperty<bool> Property_enabled { get; set; }
        public IWritableProperty<int> Property_interval { get; set; }
        public ITelemetry<double> Telemetry_workingSet { get; set; }
        public ICommand<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response> Command_getRuntimeStats { get; set; }

        private memmon(IMqttBaseClient c) : base(c)
        {
            Property_started = new ReadOnlyProperty<DateTime>(c, "started");
            Property_interval = new WritableProperty<int>(c, "interval");
            Property_enabled = new WritableProperty<bool>(c, "enabled");
            Telemetry_workingSet = new Telemetry<double>(c, "workingSet");
            Command_getRuntimeStats = new Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response>(c, "getRuntimeStats");
        }

        public static async Task<memmon> CreateClientAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            var cs = connectionString + ";ModelId=" + modelId;
            var mqtt = await new Rido.Mqtt.MqttNet4Adapter.MqttNetClientConnectionFactory().CreateHubClientAsync(cs);
            var client = new memmon(mqtt);
            client.InitialState = await client.GetTwinAsync(cancellationToken);
            return client;
        }
    }



}
