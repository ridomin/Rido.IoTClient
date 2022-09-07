using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using Rido.Mqtt.MqttNet4Adapter;
using Rido.MqttCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Rido.Mqtt.IntegrationTests
{
    public class TestMosquittoRidoDev
    {
        MqttClient? client;
        public TestMosquittoRidoDev()
        {
            client = new MqttFactory().CreateMqttClient(MqttNetTraceLogger.CreateTraceLogger()) as MqttClient;
        }


        [Fact]
        public async Task NotFailsWithouCA()
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            var cs = new ConnectionSettings()
            {
                HostName = "mosquitto.rido.dev",
                TcpPort = 8883,
                ClientId = "test-client",
                UserName = "client1",
                Password = "Pass@Word1"
            };
            
            var connAck = await client.ConnectAsync(new MqttClientOptionsBuilder()
                .WithX509Auth(cs)
                .Build());
            
        }

        //[Fact]
        //public async Task ConfiguredCA()
        //{
        //    if (client == null) throw new ArgumentNullException(nameof(client));
        //    var cs = new ConnectionSettings()
        //    {
        //        HostName = "mosquitto.rido.dev",
        //        TcpPort = 8883,
        //        CaPath = "RidoFY23CA.crt",
        //        UserName = "client1",
        //        Password = "Pass@Word1"
        //    };
        //    var connAck = await client.ConnectAsync(new MqttClientOptionsBuilder()
        //        .WithX509Auth(cs)
        //        .Build());
        //    Assert.Equal(MqttClientConnectResultCode.Success, connAck.ResultCode);
        //    Assert.True(client.IsConnected);
        //    await client.DisconnectAsync();
        //}

        [Fact]
        public async Task ClientCert()
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            var cs = new ConnectionSettings()
            {
                HostName = "mosquitto.rido.dev",
                TcpPort = 8884,
                ClientId = "test-client",
                CaPath = "RidoFY23CA.crt",
                X509Key = "mqttClient.pfx|1234"
            };
            var connAck = await client.ConnectAsync(new MqttClientOptionsBuilder()
                .WithX509Auth(cs)
                .Build());
            Assert.Equal(MqttClientConnectResultCode.Success, connAck.ResultCode);
            Assert.True(client.IsConnected);
            await client.DisconnectAsync();
        }
    }
}