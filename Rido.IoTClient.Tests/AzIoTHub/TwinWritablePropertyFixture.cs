using MQTTnet.Client;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.Tests.AzIoTHub
{
    public class TwinWritablePropertyFixture
    {
        static string Stringify(object o) => System.Text.Json.JsonSerializer.Serialize(o);

        readonly WritableProperty<double> wp;
        readonly IMqttClient connection;

        public TwinWritablePropertyFixture()
        {
            connection = new MockMqttClient();
            wp = new WritableProperty<double>(connection, "myProp");
        }

        [Fact]
        public async Task InitEmptyTwin()
        {
            string twin = Stringify(new
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
            string twin = Stringify(new
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
        public async Task InitTwinWithDesiredTriggersUpdate()
        {
            wp.OnProperty_Updated = async p =>
            {
                p.Status = 200;
                return await Task.FromResult(p);
            };
            string twin = Stringify(new
            {
                reported = new Dictionary<string, object>() { { "$version", 1 } },
                desired = new Dictionary<string, object>() { { "$version", 2 }, { "myProp", 3.1 } },
            });
            await wp.InitPropertyAsync(twin, 0.2);
            Assert.Equal(3.1, wp.PropertyValue.Value);
            Assert.Equal(200, wp.PropertyValue.Status);
            Assert.Equal(2, wp.PropertyValue.Version);
            Assert.Equal(2, wp.PropertyValue.DesiredVersion);
        }





        [Fact]
        public async Task InitTwinWithDesiredInComponent()
        {
            var wpWithComp = new WritableProperty<double>(connection, "myProp", "myComp");
            string twin = Stringify(new
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
            string twin = Stringify(new
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
            string twin = Stringify(new
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
            string twin = Stringify(new
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
            string twin = Stringify(new
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
}
