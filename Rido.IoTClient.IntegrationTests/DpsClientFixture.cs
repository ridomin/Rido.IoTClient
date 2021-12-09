using Rido.IoTClient.AzDps;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.IntegrationTests
{
    public class DpsClientFixture
    {
        [Fact]
        public async Task ProvisionWithSas()
        {
            var dpsRes = await DpsClient.ProvisionWithSasAsync("0ne003861C6", "sasdpstest", "l38DGXhjOrdYlqExavXemTBR+QqiAfus9Qp+L1HwuYA=");
            Assert.Equal("rido.azure-devices.net", dpsRes.registrationState.assignedHub);
        }

        [Fact]
        public async Task ProvisionWithSasPnP()
        {
            var dpsRes = await DpsClient.ProvisionWithSasAsync("0ne003861C6", "sasdpstest", "l38DGXhjOrdYlqExavXemTBR+QqiAfus9Qp+L1HwuYA=", "dtmi:test:dps;1");
            Assert.Equal("rido.azure-devices.net", dpsRes.registrationState.assignedHub);
        }

        [Fact]
        public async Task ProvisionWithCert()
        {
            var dpsRes = await DpsClient.ProvisionWithCertAsync("0ne003861C6", "testdevice.pfx", "1234");
            Assert.Equal("rido.azure-devices.net", dpsRes.registrationState.assignedHub);
        }
        [Fact]
        public async Task ProvisionWithCertPnP()
        {
            var dpsRes = await DpsClient.ProvisionWithCertAsync("0ne003861C6", "testdevice.pfx", "1234", "dtmi:test:dps;1");
            Assert.Equal("rido.azure-devices.net", dpsRes.registrationState.assignedHub);
        }
    }
}
