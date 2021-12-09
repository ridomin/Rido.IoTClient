using dtmi_rido_pnp_sample;
using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace dtmi_rido_pnp_sample
{
    class memMonComponent : Component<pnp_memmon_1>
    {
        public memMonComponent(IMqttClient c, string name) : base(c, name)
        {
            CV = new pnp_memmon_1();
            CV.Property_interval = new WritableProperty<int>(c, "interval", "memMon");
            CV.Property_enabled = new WritableProperty<bool>(c, "enabled", "memMon");
            CV.Property_started = new ReadOnlyProperty<DateTime>(c, "started");
            CV.Telemetry_workingSet = new Telemetry<double>(c, "workingSet", "memMon");
            CV.Command_getRuntimeStats = new Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response>(c, "getRuntimeStats", "memMon");
        }
    }

    public class pnp_memmon_1 : ITwinSerializable
    {
        public ReadOnlyProperty<DateTime> Property_started;
        public WritableProperty<bool> Property_enabled;
        public WritableProperty<int> Property_interval;
        public Telemetry<double> Telemetry_workingSet;
        public Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response> Command_getRuntimeStats;


      

        public Dictionary<string, object> ToJson()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("started", Property_started.PropertyValue);
            return dict;
        }
    }
}
