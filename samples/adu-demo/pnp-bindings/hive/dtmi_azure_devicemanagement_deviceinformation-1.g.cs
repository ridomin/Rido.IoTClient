﻿//  <auto-generated/> 
using Rido.Mqtt.Client.TopicBindings;
using Rido.MqttCore;
using Rido.MqttCore.PnP;

namespace adu_demo_pnp_bindings_hive
{

    public class DeviceInformation : Component, dtmi.azure.DeviceManagement.DeviceInformation
    {
        public DeviceInformation(IMqttConnection c, string name) : base(c, name)
        {
            Property_manufacturer = new ReadOnlyProperty<string>(c, "manufacturer");
            Property_model = new ReadOnlyProperty<string>(c, "model");
            Property_swVersion = new ReadOnlyProperty<string>(c, "swVersion");
            Property_osName = new ReadOnlyProperty<string>(c, "osName");
            Property_processorArchitecture = new ReadOnlyProperty<string>(c, "processorArchitecture");
            Property_processorManufacturer = new ReadOnlyProperty<string>(c, "processorManufacturer");
            Property_totalMemory = new ReadOnlyProperty<long>(c, "totalMemory");
            Property_totalStorage = new ReadOnlyProperty<long>(c, "totalStorage");
        }

        public IReadOnlyProperty<string> Property_manufacturer { get; set; }
        public IReadOnlyProperty<string> Property_model { get; set; }
        public IReadOnlyProperty<string> Property_swVersion { get; set; }
        public IReadOnlyProperty<string> Property_osName { get; set; }
        public IReadOnlyProperty<string> Property_processorArchitecture { get; set; }
        public IReadOnlyProperty<string> Property_processorManufacturer { get; set; }
        public IReadOnlyProperty<long> Property_totalMemory { get; set; }
        public IReadOnlyProperty<long> Property_totalStorage { get; set; }

        public override Dictionary<string, object> ToJsonDict()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add(Property_manufacturer.PropertyName, Property_manufacturer.PropertyValue);
            dic.Add(Property_model.PropertyName, Property_model.PropertyValue);
            dic.Add(Property_swVersion.PropertyName, Property_swVersion.PropertyValue);
            dic.Add(Property_osName.PropertyName, Property_osName.PropertyValue);
            dic.Add(Property_processorArchitecture.PropertyName, Property_processorArchitecture.PropertyValue);
            dic.Add(Property_processorManufacturer.PropertyName, Property_processorManufacturer.PropertyValue);
            dic.Add(Property_totalMemory.PropertyName, Property_totalMemory.PropertyValue);
            dic.Add(Property_totalStorage.PropertyName, Property_totalStorage.PropertyValue);
            return dic;
        }
    }
}