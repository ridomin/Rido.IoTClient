using Rido.MqttCore;
using System;
using Xunit;

namespace Rido.Mqtt.UnitTests
{
    public class ConnectionSettingsFixture
    {
        [Fact]
        public void DefaultValues()
        {
            var dcs = new ConnectionSettings();
            Assert.Equal(60, dcs.SasMinutes);
            Assert.Equal(60, dcs.KeepAliveInSeconds);
            Assert.Equal(AuthType.Basic, dcs.Auth);
            Assert.Equal(8883, dcs.TcpPort);
            Assert.False(dcs.DisableCrl);
            Assert.True(dcs.UseTls);
            Assert.Equal("TcpPort=8883;SasMinutes=60;Auth=Basic", dcs.ToString());
        }

        [Fact]
        public void ParseConnectionString()
        {
            string cs = "HostName=<hubname>.azure-devices.net;DeviceId=<deviceId>;SharedAccessKey=<SasKey>";
            ConnectionSettings dcs = ConnectionSettings.FromConnectionString(cs);
            Assert.Equal("<hubname>.azure-devices.net", dcs.HostName);
            Assert.Equal("<deviceId>", dcs.DeviceId);
            Assert.Equal("<SasKey>", dcs.SharedAccessKey);
            Assert.Equal(Environment.MachineName, dcs.ClientId);
        }

        [Fact]
        public void InvalidValuesDontUseDefaults()
        {
            string cs = "HostName=<hubname>.azure-devices.net;DeviceId=<deviceId>;SharedAccessKey=<SasKey>;MaxRetries=-2;SasMinutes=aa;RetryInterval=4.3";
            ConnectionSettings dcs = ConnectionSettings.FromConnectionString(cs);
            Assert.Equal("<hubname>.azure-devices.net", dcs.HostName);
            Assert.Equal("<deviceId>", dcs.DeviceId);
            Assert.Equal("<SasKey>", dcs.SharedAccessKey);
            Assert.Equal(60, dcs.SasMinutes);
        }


        [Fact]
        public void ParseConnectionStringWithDefaultValues()
        {
            string cs = "HostName=<hubname>.azure-devices.net;DeviceId=<deviceId>;ModuleId=<moduleId>;SharedAccessKey=<SasKey>";
            ConnectionSettings dcs = ConnectionSettings.FromConnectionString(cs);
            Assert.Equal("<hubname>.azure-devices.net", dcs.HostName);
            Assert.Equal("<deviceId>", dcs.DeviceId);
            Assert.Equal("<moduleId>", dcs.ModuleId);
            Assert.Equal("<SasKey>", dcs.SharedAccessKey);
            Assert.Equal(60, dcs.SasMinutes);
            Assert.Equal(60, dcs.KeepAliveInSeconds);
            Assert.Equal(8883, dcs.TcpPort);
            Assert.Equal(Environment.MachineName, dcs.ClientId);
            Assert.True(dcs.UseTls);
            Assert.False(dcs.DisableCrl);
        }

        [Fact]
        public void ParseConnectionStringWithAllValues()
        {
            string cs = "HostName=<hubname>.azure-devices.net;DeviceId=<deviceId>;ClientId=<ClientId>;ModuleId=<moduleId>;SharedAccessKey=<SasKey>;SasMinutes=2;TcpPort=1234;UseTls=false;CaPath=<path>;DisableCrl=true;UserName=<usr>;Password=<pwd>";
            ConnectionSettings dcs = ConnectionSettings.FromConnectionString(cs);
            Assert.Equal("<hubname>.azure-devices.net", dcs.HostName);
            Assert.Equal("<deviceId>", dcs.DeviceId);
            Assert.Equal("<moduleId>", dcs.ModuleId);
            Assert.Equal("<SasKey>", dcs.SharedAccessKey);
            Assert.Equal("<ClientId>", dcs.ClientId);
            Assert.Equal("<usr>", dcs.UserName);
            Assert.Equal("<pwd>", dcs.Password);
            Assert.Equal(2, dcs.SasMinutes);
            Assert.Equal(1234, dcs.TcpPort);
            Assert.False(dcs.UseTls);
            Assert.Equal("<path>", dcs.CaPath);
            Assert.True(dcs.DisableCrl);
        }

        [Fact]
        public void ToStringReturnConnectionString()
        {
            ConnectionSettings dcs = new ConnectionSettings()
            {
                HostName = "h",
                DeviceId = "d",
                SharedAccessKey = "sas",
                ModelId = "dtmi"
            };
            string expected = "HostName=h;TcpPort=8883;DeviceId=d;SharedAccessKey=***;ModelId=dtmi;SasMinutes=60;Auth=Basic";
            Assert.Equal(expected, dcs.ToString());
        }

        [Fact]
        public void ToStringReturnConnectionStringWithModule()
        {
            ConnectionSettings dcs = new ConnectionSettings()
            {
                HostName = "h",
                DeviceId = "d",
                ModuleId = "m",
                SharedAccessKey = "sas"
            };
            string expected = "HostName=h;TcpPort=8883;DeviceId=d;ModuleId=m;SharedAccessKey=***;SasMinutes=60;Auth=Basic";
            Assert.Equal(expected, dcs.ToString());
        }
    }
}
