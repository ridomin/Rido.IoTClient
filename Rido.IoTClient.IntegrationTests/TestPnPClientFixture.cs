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
            
            await client.Property_deviceInfo.UpdateTwinPropertyAsync(new DeviceInfo 
            { 
                UserName = client.Connection.Options.Credentials.Username 
            });
            var twinJson = await client.GetTwinAsync();

            var twin = JsonNode.Parse(twinJson);
            Assert.NotNull(twin);
            Assert.StartsWith(cs.HostName, twin?["reported"]?[client.Property_deviceInfo.Name]?["UserName"]?.GetValue<string>());
            Assert.Equal(Environment.MachineName, client.Property_deviceInfo.PropertyValue.MachineName);
            Assert.StartsWith(cs.HostName, client.Property_deviceInfo.PropertyValue.UserName);
            
            await client.Connection.DisconnectAsync(MqttClientDisconnectReason.NormalDisconnection);
        }
    }
}
