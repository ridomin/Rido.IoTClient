using Rido.Mqtt.AzIoTClient;
using System.Threading.Tasks;
using Xunit;

namespace Rido.Mqtt.IntergationTests
{
    public class DpsClientTests
    {


        [Fact]
        public async Task ProvisionWithSas()
        {
            string cs = "IdScope=0ne006CCDE4;DeviceId=sasdpstest;SharedAccessKey=tj9qgyxR9lm5tUYmlbtYsTFHAV/DjcbF4xrnlUeJLC4=";
            var mqtt = await new MqttNet4Adapter.MqttNetClientConnectionFactory().CreateDpsClientAsync(cs);
            var dpsClient = new MqttDpsClient(mqtt);
            var dpsRes = await dpsClient.ProvisionDeviceIdentity();
            Assert.Equal("rido.azure-devices.net", dpsRes.RegistrationState.AssignedHub);
        }

        [Fact]
        public async Task ProvisionWithCertPnP()
        {
            string cs = "IdScope=0ne006CCDE4;X509Key=testdevice22.pfx|1234;ModelId=dtmi:rido:test;1";
            var mqtt = await new MqttNet4Adapter.MqttNetClientConnectionFactory().CreateDpsClientAsync(cs);
            var dpsClient = new MqttDpsClient(mqtt);
            var dpsRes = await dpsClient.ProvisionDeviceIdentity();
            Assert.Equal("rido.azure-devices.net", dpsRes.RegistrationState.AssignedHub);
        }

        //[Fact]
        //public async Task ProvisionWithBadSasThrows()
        //{
        //    try
        //    {
        //        var dpsRes = await dpsClient.ProvisionWithSasAsync("0ne006CCDE4", "bad", "l38DGXhjOrdYlqExavXemTBR+QqiAfus9Qp+L1HwuYA=");
        //    }
        //    catch (ApplicationException ex)
        //    {
        //        ArgumentNullException.ThrowIfNull(ex.Message);
        //        Assert.Contains("Unauthorized", ex.Message);
        //    }
        //}



        //[Fact]
        //public async Task ProvisionWithSasPnP()
        //{
        //    var dpsRes = await dpsClient.ProvisionWithSasAsync("0ne006CCDE4", "sasdpstest", "l38DGXhjOrdYlqExavXemTBR+QqiAfus9Qp+L1HwuYA=", "dtmi:test:dps;1");
        //    Assert.Equal("rido.azure-devices.net", dpsRes.RegistrationState.AssignedHub);
        //}

        //[Fact]
        //public async Task ProvisionWithCert()
        //{
        //    var dpsRes = await dpsClient.ProvisionWithCertAsync("0ne006CCDE4", "testdevice22.pfx", "1234");
        //    Assert.Equal("ridox.azure-devices.net", dpsRes.RegistrationState.AssignedHub);
        //}
        //[Fact]
        //public async Task ProvisionWithCertPnP()
        //{
        //    var dpsRes = await dpsClient.ProvisionWithCertAsync("0ne006CCDE4", "testdevice22.pfx", "1234", "dtmi:test:dps;1");
        //    Assert.Equal("ridox.azure-devices.net", dpsRes.RegistrationState.AssignedHub);
        //}
    }
}