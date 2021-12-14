using System;
using System.Collections.Generic;
using System.Text;

namespace Rido.IoTClient
{
    public class GenericCommandRequest 
    {
        public string CommandName { get; set; }
        public string CommandPayload { get; set; }
    }
}
