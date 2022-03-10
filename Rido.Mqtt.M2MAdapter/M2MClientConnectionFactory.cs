using Rido.MqttCore;
using System;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace Rido.Mqtt.M2MAdapter
{
    public class M2MClientConnectionFactory : IHubClientConnectionFactory
    {
        public async Task<IMqttBaseClient> CreateHubClientAsync(string connectionSettingsString, CancellationToken cancellationToken = default)
        {
            var connectionSettings = new ConnectionSettings(connectionSettingsString);
            MqttClient mqtt = null;
            if (connectionSettings.Auth == "SAS")
            {
                mqtt = new MqttClient(connectionSettings.HostName, 8883, true, MqttSslProtocols.TLSv1_2, null, null);
                (string u, string p) = SasAuth.GenerateHubSasCredentials(connectionSettings.HostName, connectionSettings.DeviceId, connectionSettings.SharedAccessKey, connectionSettings.ModelId, connectionSettings.SasMinutes);
                int res = mqtt.Connect(connectionSettings.DeviceId, u, p);
                Console.WriteLine(res);
            } else if (connectionSettings.Auth == "X509")
            {
                var segments = connectionSettings.X509Key.Split('|');
                string pfxpath = segments[0];
                string pfxpwd = segments[1];
                var cert = new X509Certificate2(pfxpath, pfxpwd);
                string clientId = X509CommonNameParser.GetCNFromCertSubject(cert.Subject);
                connectionSettings.ClientId = clientId;
                mqtt = new MqttClient(connectionSettings.HostName, 8883, true, null, cert, MqttSslProtocols.TLSv1_2);
                mqtt.Connect(clientId, SasAuth.GetUserName(connectionSettings.HostName, clientId, connectionSettings.ModelId), string.Empty);
            }

            if (mqtt==null)
            {
                throw new SecurityException("Cannot create Mqtt client");
            }    
            return await Task.FromResult(new M2MClient(mqtt) { ConnectionSettings = connectionSettings });
        }

        public async Task<IMqttBaseClient> CreateBrokerClientAsync(string connectionSettingsString)
        {
            var connectionSettings = new ConnectionSettings(connectionSettingsString); ;
            var mqtt = new MqttClient(connectionSettings.HostName, 8883, true, MqttSslProtocols.TLSv1_2, null, null);
            int res = mqtt.Connect(connectionSettings.DeviceId, connectionSettings.DeviceId, connectionSettings.SharedAccessKey);
            Console.WriteLine(res);
            return await Task.FromResult(new M2MClient(mqtt) { ConnectionSettings = connectionSettings });
        }
    }
}
