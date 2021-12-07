﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.Tests
{
    class AComplexObj
    {
        public string AStringProp { get; set; } = String.Empty;
        public int AIntProp { get; set; }
    }

    public  class PropertyAckFixture
    {
        static string js(object o) => System.Text.Json.JsonSerializer.Serialize(o);

        [Fact]
        public void AckDouble()
        {
            var wp = new PropertyAck<double>("aDouble")
            {
                Description = "updated",
                Status = 200,
                Version = 3,
                Value = 1.2,
            };

            var expectedJson = js(new
            {
                aDouble = new
                {
                    av = 3,
                    ad = "updated",
                    ac = 200,
                    value = 1.2,
                }
            });
            Assert.Equal(expectedJson, wp.ToAck());
        }

        [Fact]
        public void AckDateTime()
        {
            var wpDate = new PropertyAck<DateTime>("aDateTime")
            {
                Value = new DateTime(2011, 11, 10, 8, 31, 12),
                Version = 3,
                Status = 200,
                Description = "updated"
            };

            var expectedJson = js(new
            {
                aDateTime = new
                {
                    av = 3,
                    ad = "updated",
                    ac = 200,
                    value = "2011-11-10T08:31:12",
                }
            });
            Assert.Equal(expectedJson, wpDate.ToAck());
        }

        [Fact]
        public void AckBool()
        {
            var wpBoolean = new PropertyAck<bool>("aBoolean")
            {
                Value = false,
                Version = 3,
                Status = 200,
                Description = "updated"
            };

            var expectedJson = js(new
            {
                aBoolean = new
                {
                    av = 3,
                    ad = "updated",
                    ac = 200,
                    value = false,
                }
            });
            Assert.Equal(expectedJson, wpBoolean.ToAck());
        }

        [Fact]
        public void AckComplexOject()
        {
            var aComplexObj = new AComplexObj() { AIntProp = 1, AStringProp = "a" };
            var prop = new PropertyAck<AComplexObj>("aComplexObj")
            {
                Version = 3,
                Value = aComplexObj,
                Status = 213,
                Description = "description"
            };
            var expectedJson = js(new
            {
                aComplexObj = new
                {
                    av = 3,
                    ad = "description",
                    ac = 213,
                    value = new
                    {
                        AStringProp = "a",
                        AIntProp = 1
                    },
                }
            });
            Assert.Equal(expectedJson, prop.ToAck());
        }

        [Fact]
        public void AckDoubleInComponent()
        {
            var wp = new PropertyAck<double>("aDouble", "inAComp")
            {
                Version = 4,
                Status = 200,
                Value = 2.4,
                Description = "updated"
            };
            var expectedJson = js(new
            {
                inAComp = new
                {
                    __t = "c",
                    aDouble = new
                    {
                        av = 4,
                        ad = "updated",
                        ac = 200,
                        value = 2.4,
                    }
                }
            });
            Assert.Equal(expectedJson, wp.ToAck());
        }
    }
}
