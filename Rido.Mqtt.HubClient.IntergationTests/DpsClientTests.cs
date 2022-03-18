using MQTTnet;
using Rido.Mqtt.DpsClient;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Rido.Mqtt.HubClient.IntergationTests
{
    public class DpsClientTests
    {
        
        
        [Fact]
        public async Task ProvisionWithSas()
        {
            string cs = "IdScope=0ne003861C6;DeviceId=sasdpstest;SharedAccessKey=l38DGXhjOrdYlqExavXemTBR+QqiAfus9Qp+L1HwuYA=";
            var mqtt = await new Rido.Mqtt.MqttNet3Adapter.MqttNetClientConnectionFactory().CreateDpsClientAsync(cs);
            var dpsClient = new MqttDpsClient(mqtt);
            var dpsRes = await dpsClient.ProvisionDeviceIdentity();
            Assert.Equal("rido.azure-devices.net", dpsRes.RegistrationState.AssignedHub);
        }

        //[Fact]
        //public async Task ProvisionWithBadSasThrows()
        //{
        //    try
        //    {
        //        var dpsRes = await dpsClient.ProvisionWithSasAsync("0ne003861C6", "bad", "l38DGXhjOrdYlqExavXemTBR+QqiAfus9Qp+L1HwuYA=");
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
        //    var dpsRes = await dpsClient.ProvisionWithSasAsync("0ne003861C6", "sasdpstest", "l38DGXhjOrdYlqExavXemTBR+QqiAfus9Qp+L1HwuYA=", "dtmi:test:dps;1");
        //    Assert.Equal("rido.azure-devices.net", dpsRes.RegistrationState.AssignedHub);
        //}

        //[Fact]
        //public async Task ProvisionWithCert()
        //{
        //    var dpsRes = await dpsClient.ProvisionWithCertAsync("0ne003861C6", "testdevice22.pfx", "1234");
        //    Assert.Equal("ridox.azure-devices.net", dpsRes.RegistrationState.AssignedHub);
        //}
        //[Fact]
        //public async Task ProvisionWithCertPnP()
        //{
        //    var dpsRes = await dpsClient.ProvisionWithCertAsync("0ne003861C6", "testdevice22.pfx", "1234", "dtmi:test:dps;1");
        //    Assert.Equal("ridox.azure-devices.net", dpsRes.RegistrationState.AssignedHub);
        //}
    }
}