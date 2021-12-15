using MQTTnet.Client;
using Rido.IoTClient.AzIoTHub;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rido.IoTClient.IntegrationTests
{
    public class Person
    {
        public string? Name { get; set; }
        public int Age { get; set; }
    }

    public class TestPnPClient : PnPClient
    {
        public readonly ReadOnlyProperty<Person> Property_person;
        private TestPnPClient(IMqttClient connection) : base(connection)
        {
            Property_person = new ReadOnlyProperty<Person>(connection, "person");
        }

        public static async Task<TestPnPClient> CreateAsync(ConnectionSettings cs)
        {
            var client = await PnPClient.CreateAsync(cs);
            return new TestPnPClient(client.Connection);
        }
    }
}
