using Rido.MqttCore;
using System;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace Rido.Mqtt.M2MAdapter
{
    public class M2MClientConnectionFactory
    {
        public async Task<IMqttBaseClient> CreateHubClientAsync(string connectionSettingsString, CancellationToken cancellationToken = default)
        {
            var cs = new ConnectionSettings(connectionSettingsString);
            MqttClient mqtt = null;
            if (cs.Auth == AuthType.Sas)
            {
                mqtt = new MqttClient(cs.HostName, cs.TcpPort, true, MqttSslProtocols.TLSv1_2, null, null);
                (string u, string p) = SasAuth.GenerateHubSasCredentials(cs.HostName, cs.DeviceId, cs.SharedAccessKey, cs.ModelId, cs.SasMinutes);
                int res = mqtt.Connect(cs.DeviceId, u, p);
                Console.WriteLine(res);
            } 
            else if (cs.Auth == AuthType.X509)
            {
                var cert = ClientCertificateLocator.Load(cs.X509Key);
                string clientId = X509CommonNameParser.GetCNFromCertSubject(cert.Subject);
                cs.ClientId = clientId;
                mqtt = new MqttClient(cs.HostName, cs.TcpPort, true, null, cert, MqttSslProtocols.TLSv1_2);
                mqtt.Connect(clientId, SasAuth.GetUserName(cs.HostName, clientId, cs.ModelId), string.Empty);
            }

            if (mqtt==null)
            {
                throw new SecurityException("Cannot create Mqtt client");
            }    
            return await Task.FromResult(new M2MClient(mqtt) { ConnectionSettings = cs });
        }

        public async Task<IMqttBaseClient> CreateBrokerClientAsync(string connectionSettingsString)
        {
            var connectionSettings = new ConnectionSettings(connectionSettingsString); ;
            var mqtt = new MqttClient(connectionSettings.HostName, 8883, true, MqttSslProtocols.TLSv1_2, null, null);
            int res = mqtt.Connect(connectionSettings.DeviceId, connectionSettings.DeviceId, connectionSettings.SharedAccessKey);
            Console.WriteLine(res);
            return await Task.FromResult(new M2MClient(mqtt) { ConnectionSettings = connectionSettings });
        }

        public Task<IMqttBaseClient> CreateDpsClientAsync(string connectionSettingsString, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
