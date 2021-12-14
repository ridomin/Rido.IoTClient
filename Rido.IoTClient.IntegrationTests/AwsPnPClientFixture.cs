using Rido.IoTClient.Aws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MQTTnet.Client;
namespace Rido.IoTClient.IntegrationTests
{
    public class AwsPnPClientFixture
    {
        [Fact]
        public async Task Connect()
        {
            ConnectionSettings cs = new()
            {
                HostName = "a38jrw6jte2l2x-ats.iot.us-west-2.amazonaws.com",
                Auth = "X509",
                ClientId = "pnpclient-tests",
                X509Key = "aws.pfx|1234"
            };
            PnPClient client = await PnPClient.CreateAsync(cs);
            Assert.True(client.Connection.IsConnected);
            await client.Connection.DisconnectAsync(new MQTTnet.Client.MqttClientDisconnectOptions() { Reason = MQTTnet.Client.MqttClientDisconnectReason.NormalDisconnection });
            Assert.False(client.Connection.IsConnected);
        }

        //[Fact]
        //public async Task PubSub()
        //{
        //    ConnectionSettings cs = new()
        //    {
        //        HostName = "a38jrw6jte2l2x-ats.iot.us-west-2.amazonaws.com",
        //        ClientId = "pnpclient-testPubSub",
        //        Auth = "X509",
        //        DeviceId = "PubSubSample",
        //        X509Key = "aws.pfx|1234"
        //    };
        //    PnPClient client = await PnPClient.CreateAsync(cs);
        //    await client.Connection.SubscribeAsync("topic_1");
        //    bool received = false;
        //    client.Connection.ApplicationMessageReceivedAsync += async m =>
        //    {
        //        received = true;
        //        await Task.Yield();
        //    };
        //    await client.Connection.PublishAsync("topic_1", "{ \"hello\" : 123}");
        //    await Task.Delay(100);
        //    Assert.True(received);
        //}

        [Fact]
        public async Task GetShadow()
        {
            ConnectionSettings cs = new()
            {
                HostName = "a38jrw6jte2l2x-ats.iot.us-west-2.amazonaws.com",
                ClientId = "test-shadow",
                DeviceId = "TheThing",
                Auth = "X509",
                X509Key = "TheThing.pfx|1234"
            };
            PnPClient client = await PnPClient.CreateAsync(cs);
            Assert.True(client.Connection.IsConnected);
            var shadow = await client.GetShadowAsync();
            Assert.NotNull(shadow);
        }
    }
}
