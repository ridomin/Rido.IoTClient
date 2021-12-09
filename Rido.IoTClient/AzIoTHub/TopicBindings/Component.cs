using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub.TopicBindings
{
    public class Component<T>
    {
        readonly string name;
        readonly UpdateTwinBinder update;

        public Component(IMqttClient connection, string name)
        {
            this.name = name;
            update = new UpdateTwinBinder(connection);
        }

        public async Task<int> UpdateTwinAsync(T thisDeviceInfo)
        {
            Dictionary<string, T> dict = new Dictionary<string, T>
            {
                { name, thisDeviceInfo }
            };
            return await update.UpdateTwinAsync(dict);
        }
    }
}
