using Rido.MqttCore;
using System.Threading.Tasks;
using Xunit;

namespace Rido.Mqtt.IntergationTests
{
    public class AwsConnectionFixture
    {
        readonly ConnectionSettings cs = new()
        {
            HostName = "a38jrw6jte2l2x-ats.iot.us-west-1.amazonaws.com",
            Auth = "X509",
            ClientId = "testdevice22",
            X509Key = "testdevice22.pfx|1234"
        };

        [Fact]
        public async Task Connect()
        {
            var client = await new MqttNet3Adapter.MqttNetClientConnectionFactory().CreateAwsClientAsync(cs);
            Assert.True(client.IsConnected);
        }

        //[Fact]
        //public async Task PubSub()
        //{
        //    IMqttClient connection = new MqttFactory().CreateMqttClient();
        //    await connection.ConnectAsync(new MqttClientOptionsBuilder().WithAwsX509Credentials(cs).Build());

        //    await connection.SubscribeAsync("topic_1");
        //    bool received = false;
        //    connection.ApplicationMessageReceivedAsync += async m =>
        //    {
        //        received = true;
        //        await Task.Yield();
        //    };
        //    await connection.PublishAsync("topic_1", "{ \"hello\" : 123}");
        //    await Task.Delay(100);
        //    Assert.True(received);
        //}

        //[Fact]
        //public async Task GetShadow()
        //{
        //    var client = new AwsClient(await AwsConnectionFactory.CreateAsync(cs));
        //    Assert.True(client.Connection.IsConnected);
        //    var shadow = await client.GetShadowAsync();
        //    Assert.NotNull(shadow);
        //}

        //[Fact]
        //public async Task UpdateShadow()
        //{
        //    var client = new AwsClient(await AwsConnectionFactory.CreateAsync(cs));
        //    Assert.True(client.Connection.IsConnected);
        //    var shadow = await client.GetShadowAsync();
        //    Assert.NotNull(shadow);
        //    var updRes = await client.UpdateShadowAsync(new
        //    {
        //        name = "rido2"
        //    });
        //    Assert.True(updRes > 0);
        //}

        //[Fact]
        //public async Task UpdateShadowConcurrent()
        //{
        //    var client = new AwsClient(await AwsConnectionFactory.CreateAsync(cs));
        //    Assert.True(client.Connection.IsConnected);
        //    var shadow = await client.GetShadowAsync();
        //    Assert.NotNull(shadow);
        //    var updRes = await client.UpdateShadowAsync(new {name = "rido"});
        //    var updRes2 = await client.UpdateShadowAsync(new{name = "rido2"});
        //    Assert.True(updRes > 0);
        //    Assert.True(updRes2 > updRes);
        //}


        //[Fact]
        //public async Task ReceiveShadowUpdate()
        //{
        //    ConnectionSettings cs = new()
        //    {
        //        HostName = "a38jrw6jte2l2x-ats.iot.us-west-2.amazonaws.com",
        //        ClientId = "test-shadow",
        //        DeviceId = "TheThing",
        //        Auth = "X509",
        //        X509Key = "TheThing.pfx|1234"
        //    };
        //    PnPClient client = await PnPClient.CreateAsync(cs);
        //    bool received = false;
        //    client.desiredUpdatePropertyBinder.OnProperty_Updated = async m =>
        //    {
        //        received = true;
        //        return await Task.FromResult(new PropertyAck<string>("name")
        //        {
        //            Status = 200,
        //            Version = m.Version,
        //            Value = m.Value

        //        });
        //    };
        //    Assert.True(client.Connection.IsConnected);
        //    var updRes = await client.UpdateShadowAsync(new
        //    {
        //        name = "rido2"
        //    });
        //    Assert.True(received);
        //}


    }
}
