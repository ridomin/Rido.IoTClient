using MQTTnet;
using MQTTnet.Client;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.Tests
{
    public class TwinWritablePropertyFixture
    {
        static string js(object o) => System.Text.Json.JsonSerializer.Serialize(o);

        WritableProperty<double> wp;
        IMqttClient connection;

        public TwinWritablePropertyFixture()
        {
            connection = new MockMqttClient();
            wp = new WritableProperty<double>(connection, "myProp");
        }

        [Fact]
        public async Task InitEmptyTwin()
        {
            string twin = js(new
            {
                reported = new Dictionary<string, object>() { { "$version", 1 } },
                desired = new Dictionary<string, object>() { { "$version", 1 } },
            });

            await wp.InitPropertyAsync(twin, 0.2);
            Assert.Equal(0.2, wp.PropertyValue.Value);
            Assert.Equal(0, wp.PropertyValue.Version);
            Assert.Equal(201, wp.PropertyValue.Status);
        }

        [Fact]
        public async Task InitTwinWithReported()
        {
            string twin = js(new
            {
                reported = new
                {
                    myProp = new
                    {
                        ac = 201,
                        av = 1,
                        value = 4.3
                    }
                },
                desired = new Dictionary<string, object>() { { "$version", 1 } },
            });

            await wp.InitPropertyAsync(twin, 0.2);
            Assert.Equal(4.3, wp.PropertyValue.Value);
            Assert.Equal(1, wp.PropertyValue.Version);
            Assert.Equal(201, wp.PropertyValue.Status);
        }

        [Fact]
        public async Task InitTwinWithDesired()
        {
            string twin = js(new
            {
                reported = new Dictionary<string, object>() { { "$version", 1 } },
                desired = new Dictionary<string, object>() { { "$version", 2 }, { "myProp", 3.1 } },
            });

            await wp.InitPropertyAsync(twin, 0.2);
            Assert.Equal(3.1, wp.PropertyValue.Value);
            Assert.Equal(2, wp.PropertyValue.DesiredVersion);
        }

        

        

        [Fact]
        public async Task InitTwinWithDesiredInComponent()
        {
            var wpWithComp = new WritableProperty<double>(connection, "myProp", "myComp");
            string twin = js(new
            {
                reported = new Dictionary<string, object>() { { "$version", 1 } },
                desired = new Dictionary<string, object>() {
                    {
                        "$version", 2
                    },
                    {

                        "myComp", new Dictionary<string, object>() {
                            {
                                "__t", "c"
                            },
                            {
                                "myProp", 3.4
                            }
                        }
                    }
                }
            });
            await wpWithComp.InitPropertyAsync(twin, 0.2);
            Assert.Equal(3.4, wpWithComp.PropertyValue.Value);
            Assert.Null(wpWithComp.PropertyValue.Description);
            Assert.Equal(0, wpWithComp.PropertyValue.Status);
            Assert.Equal(2, wpWithComp.PropertyValue.DesiredVersion);
        }

        [Fact]
        public async Task InitTwinWithReportedInComponent()
        {
            var wpWithComp = new WritableProperty<double>(connection, "myProp", "myComp");
            string twin = js(new
            {
                desired = new Dictionary<string, object>()
                {
                    { "$version", 3 }
                },
                reported = new Dictionary<string, object>() 
                {
                    {
                        "$version", 4
                    },
                    {
                        "myComp", new
                        {
                            __t = "c",
                            myProp = new
                            {
                                ac = 200,
                                av = 3 ,
                                ad = "desc",
                                value = 3.4
                            }
                        }
                    }
                }
            });


            await wpWithComp.InitPropertyAsync(twin, 0.2);
            Assert.Equal(3.4, wpWithComp.PropertyValue.Value);
            Assert.Equal("desc", wpWithComp.PropertyValue.Description);
            Assert.Equal(200, wpWithComp.PropertyValue.Status);
            Assert.Equal(0, wpWithComp.PropertyValue.DesiredVersion);
        }

        [Fact]
        public async Task InitTwinWithReportedInComponentWithoutFlag()
        {
            var wpWithComp = new WritableProperty<double>(connection, "myProp", "myComp");
            string twin = js(new
            {
                desired = new Dictionary<string, object>()
                {
                    { "$version", 3 }
                },
                reported = new Dictionary<string, object>()
                {
                    {
                        "$version", 4
                    },
                    {
                        "myComp", new
                        {
                            myProp = new
                            {
                                ac = 200,
                                av = 3 ,
                                ad = "desc",
                                value = 3.4
                            }
                        }
                    }
                }
            });

            await wpWithComp.InitPropertyAsync(twin, 0.2);
            Assert.Equal(0.2, wpWithComp.PropertyValue.Value);
            Assert.Equal("Init from default value", wpWithComp.PropertyValue.Description);
            Assert.Equal(201, wpWithComp.PropertyValue.Status);
            Assert.Null(wpWithComp.PropertyValue.DesiredVersion);
        }

        [Fact]
        public async Task InitReportedDoubleInComponent()
        {
            var wpWithComp = new WritableProperty<double>(connection, "myProp", "myComp");
            string twin = js(new
            {
                reported = new
                {
                    myComp = new
                    {
                        __t = "c",
                        myProp = new
                        {
                            ac = 201,
                            av = 1,
                            value = 4.3
                        }
                    }
                },
                desired = new Dictionary<string, object>() { { "$version", 1 } },
            });

            await wpWithComp.InitPropertyAsync(twin, 0.2);
            Assert.Equal(4.3, wpWithComp.PropertyValue.Value);
            Assert.Equal(1, wpWithComp.PropertyValue.Version);
            Assert.Equal(201, wpWithComp.PropertyValue.Status);
        }

        [Fact]
        public async Task InitReportedDoubleInComponentWithouFlag()
        {
            var wpWithComp = new WritableProperty<double>(connection, "myProp", "myComp");
            string twin = js(new
            {
                reported = new
                {
                    myComp = new
                    {
                        myProp = new
                        {
                            ac = 201,
                            av = 1,
                            value = 4.3
                        }
                    }
                },
                desired = new Dictionary<string, object>() { { "$version", 1 } },
            });

            await wpWithComp.InitPropertyAsync(twin, 0.2);
            Assert.Equal(0.2, wpWithComp.PropertyValue.Value);
            Assert.Equal(0, wpWithComp.PropertyValue.Version);
            Assert.Equal(201, wpWithComp.PropertyValue.Status);
        }

    }
   

    class MockMqttClient : IMqttClient
    {
        public MockMqttClient()
        {
         
        }

        public bool IsConnected => throw new NotImplementedException();

        public IMqttClientOptions Options => throw new NotImplementedException();

        public IMqttClientConnectedHandler ConnectedHandler { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IMqttClientDisconnectedHandler DisconnectedHandler { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event Func<MqttClientConnectedEventArgs, Task> ConnectedAsync;
        public event Func<MqttClientDisconnectedEventArgs, Task> DisconnectedAsync;
        public event Func<MqttApplicationMessageReceivedEventArgs, Task> ApplicationMessageReceivedAsync;

        public Task<MqttClientConnectResult> ConnectAsync(IMqttClientOptions options, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DisconnectAsync(MqttClientDisconnectOptions options, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task PingAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<MqttClientPublishResult> PublishAsync(MqttApplicationMessage applicationMessage, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SendExtendedAuthenticationExchangeDataAsync(MqttExtendedAuthenticationExchangeData data, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<MqttClientSubscribeResult> SubscribeAsync(MqttClientSubscribeOptions options, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new MqttClientSubscribeResult());
            //throw new NotImplementedException();
        }

        public Task<MqttClientUnsubscribeResult> UnsubscribeAsync(MqttClientUnsubscribeOptions options, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
