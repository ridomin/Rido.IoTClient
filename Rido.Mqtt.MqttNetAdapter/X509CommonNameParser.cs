using System;
using System.Collections.Generic;
using System.Text;

namespace Rido.Mqtt.MqttNetAdapter
{
    internal static class X509CommonNameParser
    {
        internal static string GetCNFromCertSubject(string subject)
        {
            var result = subject[3..];
            if (subject.Contains(','))
            {
                var posComma = result.IndexOf(',');
                result = result[..posComma];
            }
            return result.Replace(" ", "");
        }
    }
}
