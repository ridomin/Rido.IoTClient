using MQTTnet.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Rido.IoTClient
{
    internal class MqttNetTraceLogger
    {
        public static MqttNetEventLogger CreateTraceLogger()
        {
            var logger = new MqttNetEventLogger();
            logger.LogMessagePublished += (s, e) =>
            {
                var trace = $">> [{e.LogMessage.Timestamp:O}] [{e.LogMessage.ThreadId}]: {e.LogMessage.Message}";
                if (e.LogMessage.Exception != null)
                {
                    trace += Environment.NewLine + e.LogMessage.Exception.ToString();
                }

                Trace.TraceInformation(trace);
            };
            return logger;
        }
    }
}
