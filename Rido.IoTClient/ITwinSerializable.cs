using System;
using System.Collections.Generic;
using System.Text;

namespace Rido.IoTClient
{
    public interface ITwinSerializable
    {
        public Dictionary<string, object> ToJson();
    }
}
