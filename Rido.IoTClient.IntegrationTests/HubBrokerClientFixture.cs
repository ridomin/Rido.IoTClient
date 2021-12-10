using MQTTnet;
using MQTTnet.Client;
using Rido.IoTClient.AzBroker;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.IntegrationTests
{
    public class HubBrokerClientFixture
    {
        readonly long tick = Environment.TickCount64;
        readonly string hostname = "broker.azure-devices.net";
        readonly string deviceId = "d5";
        //readonly string moduleId = "m1";
        readonly string defaultKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.Empty.ToString("N")));

        [Fact]
        public async Task ConnectDeviceWithSas()
        {
            var cs = new ConnectionSettings()
            {
                HostName = hostname,
                DeviceId = deviceId,
                SharedAccessKey = defaultKey
            };
            var hubClient = await HubBrokerClient.CreateAsync(cs);
            Assert.True(hubClient.Connection.IsConnected);
            Assert.Equal(deviceId, hubClient.Connection.Options.ClientId);
            var v = await hubClient.UpdateTwinAsync(new { testProp = tick });
            var twin = await hubClient.GetTwinAsync();
            Assert.Contains(tick.ToString(), twin);
            Assert.Contains(v.ToString(), twin);
            await hubClient.Connection.DisconnectAsync(new MqttClientDisconnectOptions()
            {
                Reason = MqttClientDisconnectReason.NormalDisconnection
            });
            Assert.False(hubClient.Connection.IsConnected);
        }

        //[Fact]
        //public async Task ConnectDeviceModuleWithSas()
        //{
        //    var cs = new ConnectionSettings()
        //    {
        //        HostName = hostname,
        //        DeviceId = deviceId,
        //        ModuleId = moduleId,
        //        SharedAccessKey = defaultKey
        //    };
        //    var hubClient = await HubBrokerClient.CreateAsync(cs);
        //    Assert.True(hubClient.Connection.IsConnected);
        //    Assert.Equal($"{deviceId}/{moduleId}", hubClient.Connection.Options.ClientId);
        //    var v = await hubClient.UpdateTwinAsync(new { testProp = tick });
        //    var twin = await hubClient.GetTwinAsync();
        //    Assert.Contains(tick.ToString(), twin);
        //    Assert.Contains(v.ToString(), twin);
        //    await hubClient.Connection.DisconnectAsync(new MqttClientDisconnectOptions()
        //    {
        //        Reason = MqttClientDisconnectReason.NormalDisconnection
        //    });
        //    Assert.False(hubClient.Connection.IsConnected);
        //}

        [Fact]
        public async Task ConnectDeviceWithX509()
        {
            var csx = new ConnectionSettings()
            {
                HostName = hostname,
                Auth = "X509",
                X509Key = "testdevice.pfx|1234"
            };
            var hubClient = await HubBrokerClient.CreateAsync(csx);
            Assert.True(hubClient.Connection.IsConnected);
            Assert.Equal("testdevice", hubClient.Connection.Options.ClientId);
            //var v = await hubClient.UpdateTwinAsync(new { testProp = tick });
            var twin = await hubClient.GetTwinAsync();
            Assert.True(twin.Length > 0);
            //Assert.Contains(tick.ToString(), twin);
            //Assert.Contains(v.ToString(), twin);
            await hubClient.Connection.DisconnectAsync(new MqttClientDisconnectOptions()
            {
                Reason = MqttClientDisconnectReason.NormalDisconnection
            });
            Assert.False(hubClient.Connection.IsConnected);
        }

        //[Fact]
        //public async Task ConnectDeviceModuleWithX509()
        //{
        //    var csx = new ConnectionSettings()
        //    {
        //        HostName = hostname,
        //        Auth = "X509",
        //        X509Key = "xd01_xmod01.pfx|1234"
        //    };
        //    var hubClient = await HubBrokerClient.CreateAsync(csx);
        //    Assert.True(hubClient.Connection.IsConnected);
        //    Assert.Equal("xd01/xmod01", hubClient.Connection.Options.ClientId);
        //    var v = await hubClient.UpdateTwinAsync(new { testProp = tick });
        //    var twin = await hubClient.GetTwinAsync();
        //    Assert.Contains(tick.ToString(), twin);
        //    Assert.Contains(v.ToString(), twin);
        //    await hubClient.Connection.DisconnectAsync(new MqttClientDisconnectOptions()
        //    {
        //        Reason = MqttClientDisconnectReason.NormalDisconnection
        //    });
        //    Assert.False(hubClient.Connection.IsConnected);
        //}
    }
}