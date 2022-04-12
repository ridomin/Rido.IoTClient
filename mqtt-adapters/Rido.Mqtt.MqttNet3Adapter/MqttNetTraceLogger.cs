using MQTTnet.Diagnostics;
using MQTTnet.Diagnostics.Logger;
using System;
using System.Diagnostics;

namespace Rido.Mqtt.MqttNet3Adapter
{
    public class MqttNetTraceLogger
    {
        [DebuggerStepThrough()]
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
