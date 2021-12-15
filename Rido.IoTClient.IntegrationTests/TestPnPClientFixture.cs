using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.IntegrationTests
{
    public class TestPnPClientFixture : IDisposable
    {
        readonly TestPnPClient client;
        private bool disposedValue;

        public TestPnPClientFixture()
        {
            var cs = new ConnectionSettings()
            {
                HostName = "tests.azure-devices.net",
                DeviceId = "d5",
                SharedAccessKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.Empty.ToString("N")))
            };
            client = TestPnPClient.CreateAsync(cs).Result;
        }


        [Fact]
        public async Task ValidateReadOnlyProperty()
        {
            await client.Property_person.UpdateTwinPropertyAsync(new Person { Name = "rido", Age = 33 });
            var twinJson = await client.GetTwinAsync();
            var twin = JsonNode.Parse(twinJson);
            Assert.NotNull(twin);
            Assert.Equal("rido", twin?["reported"]?[client.Property_person.Name]?["Name"]?.GetValue<string>());
            Assert.Equal("rido", client.Property_person.PropertyValue.Name);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    client.Connection.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
