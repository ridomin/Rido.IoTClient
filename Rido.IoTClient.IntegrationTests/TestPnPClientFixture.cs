using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;
using Rido.IoTClient;

namespace Rido.IoTClient.IntegrationTests
{
    public class TestPnPClientFixture 
    {
        ConnectionSettings cs = new ConnectionSettings()
        {
            HostName = "tests.azure-devices.net",
            DeviceId = "d10",
            SharedAccessKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.Empty.ToString("N")))
        };

        [Fact]
        public async Task ValidateReadOnlyProperty()
        {
            var client = await TestPnPClient.CreateAsync(cs);
            await client.Property_person.UpdateTwinPropertyAsync(new Person { Name = "rido", Age = 33 });
            var twinJson = await client.GetTwinAsync();
            var twin = JsonNode.Parse(twinJson);
            Assert.NotNull(twin);
            Assert.Equal("rido", twin?["reported"]?[client.Property_person.Name]?["Name"]?.GetValue<string>());
            Assert.Equal("rido", client.Property_person.PropertyValue.Name);
            await client.Connection.DisconnectAsync(MqttClientDisconnectReason.NormalDisconnection);
        }
    }
}
