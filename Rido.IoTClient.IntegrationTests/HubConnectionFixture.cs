using Rido.IoTClient.AzIoTHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.IntegrationTests
{
    public class HubConnectionFixture
    {
        [Fact(Skip="expired cert")]
        public async Task ConnectWithDpsCert()
        {
            var hub = await IoTHubConnectionFactory.CreateAsync(new ConnectionSettings
            {
                HostName = "ridox.azure-devices.net",
                Auth = "X509",
                X509Key = "rido-test-01.pfx|1234"
            });

            Assert.True(hub.IsConnected);

            var client = new GenericHubClient(hub);
            var twin = await client.GetTwinAsync();
            Console.WriteLine(twin);
        }
    }
}
