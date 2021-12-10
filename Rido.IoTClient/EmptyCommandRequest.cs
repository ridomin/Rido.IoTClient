using System;
using System.Collections.Generic;
using System.Text;

namespace Rido.IoTClient
{
    public class EmptyCommandRequest : IBaseCommandRequest<EmptyCommandRequest>
    {
        public EmptyCommandRequest DeserializeBody(string payload)
        {
            return new EmptyCommandRequest();
        }
    }
}
