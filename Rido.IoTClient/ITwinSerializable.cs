using System.Collections.Generic;

namespace Rido.IoTClient
{
    public interface ITwinSerializable
    {
        public Dictionary<string, object> ToJsonDict();
    }
}
