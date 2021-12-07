using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Hive.TopicBindings
{
    public class WritableProperty<T>
    {
        public PropertyAck<T> PropertyValue;
        readonly string propertyName;
        readonly string componentName;
        //readonly UpdateTwinBinder updateTwin;
        readonly UpdatePropertyBinder updatePropertyBinder;
        readonly DesiredUpdatePropertyBinder<T> desiredBinder;

        public Func<PropertyAck<T>, Task<PropertyAck<T>>> OnProperty_Updated
        {
            get => desiredBinder.OnProperty_Updated;
            set => desiredBinder.OnProperty_Updated = value;
        }

        public WritableProperty(IMqttClient connection, string name, string component = "")
        {
            propertyName = name;
            componentName = component;
            //updateTwin = new UpdateTwinBinder(connection);
            updatePropertyBinder = new UpdatePropertyBinder(connection);
            PropertyValue = new PropertyAck<T>(name, componentName);
            desiredBinder = new DesiredUpdatePropertyBinder<T>(connection, name, componentName);
        }

        public async Task UpdatePropertyAsync() => await updatePropertyBinder.ReportProperty(this.PropertyValue.ToAck());

        public async Task InitPropertyAsync(string twin, T defaultValue, CancellationToken cancellationToken = default)
        {
            PropertyValue = PropertyAck<T>.InitFromTwin(twin, propertyName, componentName, defaultValue);
            if (desiredBinder.OnProperty_Updated != null && (PropertyValue.DesiredVersion > 1))
            {
                var ack = await desiredBinder.OnProperty_Updated.Invoke(PropertyValue);
                //_ = updateTwin.UpdateTwinAsync(ack.ToAck(), cancellationToken);
                _ = updatePropertyBinder.ReportProperty(ack.ToAck(), cancellationToken);
                PropertyValue = ack;
            }
            else
            {
                _ = updatePropertyBinder.ReportProperty(PropertyValue.ToAck());
            }
        }
    }
}
