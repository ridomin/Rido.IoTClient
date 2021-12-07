using System;
using System.Web;

namespace Rido.IoTClient.AzIoTHub
{
    public class TopicParser
    {
        public static (int rid, int twinVersion) ParseTopic(string topic)
        {
            var segments = topic.Split('/');
            int twinVersion = -1;
            int rid = -1;
            if (topic.Contains('?'))
            {
                var qs = HttpUtility.ParseQueryString(segments[^1]);
                int.TryParse(qs["$rid"], out rid);
                twinVersion = Convert.ToInt32(qs["$version"]);
            }
            return (rid, twinVersion);
        }
    }
}
