using System.Linq;

namespace Rido.MqttCore
{
    /// <summary>
    /// Finds device id from X509 Common Name
    /// </summary>
    public static class X509CommonNameParser
    {
        /// <summary>
        /// Parses CN
        /// </summary>
        /// <param name="subject">Cert subject</param>
        /// <returns></returns>
        public static string GetCNFromCertSubject(string subject)
        {
            var result = subject.Substring(3);
            if (subject.Contains(','))
            {
                var posComma = result.IndexOf(',');
                result = result.Substring(0, result.Length - posComma);
            }
            return result.Replace(" ", "");
        }
    }
}
