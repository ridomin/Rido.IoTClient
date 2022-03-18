using Rido.MqttCore;
using Xunit;

namespace Rido.Mqtt.HubClient.Tests
{
    public class ConnectionSettingsFixture
    {
        [Fact]
        public void DefaultValues()
        {
            var dcs = new ConnectionSettings();
            Assert.Equal(60, dcs.SasMinutes);
            Assert.Equal(5, dcs.RetryInterval);
            Assert.Equal(10, dcs.MaxRetries);
            Assert.Equal("SAS", dcs.Auth);
            Assert.Equal("SasMinutes=60;RetryInterval=5;MaxRetries=10;Auth=SAS", dcs.ToString());
        }

        [Fact]
        public void ParseConnectionString()
        {
            string cs = "HostName=<hubname>.azure-devices.net;DeviceId=<deviceId>;SharedAccessKey=<SasKey>";
            ConnectionSettings dcs = ConnectionSettings.FromConnectionString(cs);
            Assert.Equal("<hubname>.azure-devices.net", dcs.HostName);
            Assert.Equal("<deviceId>", dcs.DeviceId);
            Assert.Equal("<SasKey>", dcs.SharedAccessKey);
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
            Assert.Equal(5, dcs.RetryInterval);
            Assert.Equal(-2, dcs.MaxRetries);
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
            Assert.Equal(10, dcs.MaxRetries);
            Assert.Equal(60, dcs.SasMinutes);
            Assert.Equal(5, dcs.RetryInterval);
        }

        [Fact]
        public void ParseConnectionStringWithAllValues()
        {
            string cs = "HostName=<hubname>.azure-devices.net;DeviceId=<deviceId>;ModuleId=<moduleId>;SharedAccessKey=<SasKey>;RetryInterval=2;MaxRetries=2;SasMinutes=2";
            ConnectionSettings dcs = ConnectionSettings.FromConnectionString(cs);
            Assert.Equal("<hubname>.azure-devices.net", dcs.HostName);
            Assert.Equal("<deviceId>", dcs.DeviceId);
            Assert.Equal("<moduleId>", dcs.ModuleId);
            Assert.Equal("<SasKey>", dcs.SharedAccessKey);
            Assert.Equal(2, dcs.MaxRetries);
            Assert.Equal(2, dcs.SasMinutes);
            Assert.Equal(2, dcs.RetryInterval);
        }

        [Fact]
        public void ToStringReturnConnectionString()
        {
            ConnectionSettings dcs = new()
            {
                HostName = "h",
                DeviceId = "d",
                SharedAccessKey = "sas",
                ModelId = "dtmi"
            };
            string expected = "HostName=h;DeviceId=d;SharedAccessKey=***;ModelId=dtmi;SasMinutes=60;RetryInterval=5;MaxRetries=10;Auth=SAS";
            Assert.Equal(expected, dcs.ToString());
        }

        [Fact]
        public void ToStringReturnConnectionStringWithModule()
        {
            ConnectionSettings dcs = new()
            {
                HostName = "h",
                DeviceId = "d",
                ModuleId = "m",
                SharedAccessKey = "sas"
            };
            string expected = "HostName=h;DeviceId=d;ModuleId=m;SharedAccessKey=***;SasMinutes=60;RetryInterval=5;MaxRetries=10;Auth=SAS";
            Assert.Equal(expected, dcs.ToString());
        }
    }
}
