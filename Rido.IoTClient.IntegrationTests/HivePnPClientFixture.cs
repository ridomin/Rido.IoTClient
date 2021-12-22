using MQTTnet;
using MQTTnet.Client;
using Rido.IoTClient.PnPMqtt;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.IntegrationTests
{
    public class HivePnPClientFixture
    {
        readonly long tick = Environment.TickCount64;
        readonly string hostname = "f8826e3352314ca98102cfbde8aff20e.s2.eu.hivemq.cloud";
        readonly string deviceId = "client1";
        readonly string defaultKey = "Myclientpwd.000";

        async Task<PnPClient> CreateHiveClient()
        {
            var cs = new ConnectionSettings()
            {
                HostName = hostname,
                DeviceId = deviceId,
                SharedAccessKey = defaultKey
            };
            return await CreateAsync(cs);
        }

        static async Task<PnPClient> CreateAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            IMqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient();
            var connAck = await mqtt.ConnectAsync(new MqttClientOptionsBuilder().WithBasicAuth(cs).Build(), cancellationToken);
            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }
            return new PnPClient(mqtt)
            {
                ConnectionSettings = cs
            };
        }

        [Fact]
        public async Task ConnectToHive()
        {
            var hiveClient = await CreateHiveClient();
            Assert.True(hiveClient.Connection.IsConnected);
            var connAck = await hiveClient.Connection.SubscribeAsync("pnp/testtopic", MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce);
            connAck.Items.ToList().ForEach(x => Assert.Equal(MqttClientSubscribeResultCode.GrantedQoS0, x.ResultCode));
            bool msgReceived = false;
            hiveClient.Connection.ApplicationMessageReceivedAsync += async m =>
            {
                msgReceived = true;
                Assert.Equal(tick, Convert.ToInt64(Encoding.UTF8.GetString(m.ApplicationMessage.Payload)));
                await Task.Yield();
            };
            await hiveClient.Connection.PublishAsync("pnp/testtopic", tick);
            await Task.Delay(500);
            Assert.True(msgReceived);
        }
    }
}
