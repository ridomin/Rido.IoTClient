﻿//  <auto-generated/> 
using Rido.Mqtt.HubClient.TopicBindings;
using Rido.MqttCore.PnP;

namespace dtmi_rido_pnp
{
    public interface IdeviceInformation : IComponent
    {
        ReadOnlyProperty<string> Property_manufacturer { get; set; }
        ReadOnlyProperty<string> Property_model { get; set; }
        ReadOnlyProperty<string> Property_osName { get; set; }
        ReadOnlyProperty<string> Property_processorArchitecture { get; set; }
        ReadOnlyProperty<string> Property_processorManufacturer { get; set; }
        ReadOnlyProperty<string> Property_swVersion { get; set; }
        ReadOnlyProperty<long> Property_totalMemory { get; set; }
        ReadOnlyProperty<long> Property_totalStorage { get; set; }
    }
}