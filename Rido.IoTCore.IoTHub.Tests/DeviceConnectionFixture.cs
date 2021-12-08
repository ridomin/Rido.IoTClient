using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTCore.IoTHub.Tests
{
    public class DeviceConnectionFixture
    {
        readonly long tick = Environment.TickCount64;
        readonly string hostname = "tests.azure-devices.net";
        readonly  string defaultKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.Empty.ToString("N")));

        [Fact]
        public async Task ConnectDeviceWithSas()
        {
            var cs = new ConnectionSettings()
            {
                HostName = hostname,
                DeviceId = "d5",
                SharedAccessKey = defaultKey
            };
            var device = await SampleDevice.CreateAsync(cs);
            Assert.True(device.Connection.IsConnected);
            Assert.Equal("d5", device.Connection.Options.ClientId);
            var v = await device.UpdateTwinAsync(new { testProp = tick });
            var twin = await device.GetTwinAsync();
            Assert.Contains(tick.ToString(), twin);
            await device.Connection.DisconnectAsync(new MqttClientDisconnectOptions()
            {
                Reason = MqttClientDisconnectReason.NormalDisconnection
            });
            Assert.False(device.Connection.IsConnected);
        }

        [Fact]
        public async Task ConnectDeviceModuleWithSas()
        {
            var cs = new ConnectionSettings()
            {
                HostName = hostname,
                DeviceId = "d5",
                ModuleId = "m1",
                SharedAccessKey = defaultKey
            };
            var device = await SampleDevice.CreateAsync(cs);
            Assert.True(device.Connection.IsConnected);
            Assert.Equal("d5/m1", device.Connection.Options.ClientId);
            var v = await device.UpdateTwinAsync(new { testProp = tick });
            var twin = await device.GetTwinAsync();
            Assert.Contains(tick.ToString(), twin);
            await device.Connection.DisconnectAsync(new MqttClientDisconnectOptions()
            {
                Reason = MqttClientDisconnectReason.NormalDisconnection
            });
            Assert.False(device.Connection.IsConnected);
        }

        [Fact]
        public async Task ConnectDeviceWithX509()
        {
            var csx = new ConnectionSettings()
            {
                HostName = "tests.azure-devices.net",
                Auth = "X509",
                X509Key = "testdevice.pfx|1234"
            };
            var device = await SampleDevice.CreateAsync(csx);
            Assert.True(device.Connection.IsConnected);
            Assert.Equal("testdevice", device.Connection.Options.ClientId);
            var v = await device.UpdateTwinAsync(new { testProp = tick });
            var twin = await device.GetTwinAsync();
            Assert.Contains(tick.ToString(), twin);
            await device.Connection.DisconnectAsync(new MqttClientDisconnectOptions() 
            { 
                Reason = MqttClientDisconnectReason.NormalDisconnection 
            });
            Assert.False(device.Connection.IsConnected);
        }

        [Fact]
        public async Task ConnectDeviceModuleWithX509()
        {
            var csx = new ConnectionSettings()
            {
                HostName = "tests.azure-devices.net",
                Auth = "X509",
                X509Key = "xd01_xmod01.pfx|1234"
            };
            var device = await SampleDevice.CreateAsync(csx);
            Assert.True(device.Connection.IsConnected);
            Assert.Equal("xd01/xmod01", device.Connection.Options.ClientId);
            var v = await device.UpdateTwinAsync(new { testProp = tick });
            var twin = await device.GetTwinAsync();
            Assert.Contains(tick.ToString(), twin);
            await device.Connection.DisconnectAsync(new MqttClientDisconnectOptions()
            {
                Reason = MqttClientDisconnectReason.NormalDisconnection
            });
            Assert.False(device.Connection.IsConnected);
        }
    }
}