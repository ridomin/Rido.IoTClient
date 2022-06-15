using MQTTnet;
using MQTTnet.Client;
using Rido.MqttCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.MqttNet4Adapter
{
    public class MqttNetClientConnectionFactory : IHubClientConnectionFactory
    {
        MqttNetClient managedClient;
        Timer reconnectTimer;
        public async Task<IMqttBaseClient> CreateHubClientAsync(string connectionSettingsString, CancellationToken cancellationToken = default)
        {
            var cs = new ConnectionSettings(connectionSettingsString);
            return await CreateHubClientAsync(cs);
        }

        public async Task<IMqttBaseClient> CreateHubClientAsync(ConnectionSettings connectionSettings, CancellationToken cancellationToken = default)
        {
            IMqttClient connection = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient();
            MqttClientConnectResult connAck;

            if (connectionSettings.Auth == AuthType.Sas)
            {
                connAck = ConnectWithTimer(connection, connectionSettings);
            }
            else
            {
                connAck = await connection.ConnectAsync(
                    new MqttClientOptionsBuilder()
                        .WithAzureIoTHubCredentials(connectionSettings)
                        .WithKeepAlivePeriod(TimeSpan.FromSeconds(connectionSettings.KeepAliveInSeconds))
                        .Build(),
                    cancellationToken);
            }
            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }
            managedClient = new MqttNetClient(connection) { ConnectionSettings = connectionSettings };
            return managedClient;
        }
        
        private MqttClientConnectResult ConnectWithTimer(IMqttClient connection, ConnectionSettings connectionSettings)
        {
            if (connection.IsConnected)
            {
                Trace.TraceWarning("Reconnecting for new SasToken");
                connection.DisconnectAsync().Wait();
            }
            var connAck = connection.ConnectAsync(
                new MqttClientOptionsBuilder()
                    .WithAzureIoTHubCredentials(connectionSettings)
                    .WithKeepAlivePeriod(TimeSpan.FromSeconds(connectionSettings.KeepAliveInSeconds))
                    .Build()).Result;

            if (managedClient!=null && managedClient.subscriptions.Count>0)
            {
                foreach (var s in managedClient.subscriptions)
                {
                    connection.SubscribeAsync(s).Wait();
                }
            }    

            reconnectTimer = new Timer(o =>
            {
                connAck = ConnectWithTimer(connection, connectionSettings);
            }, null, (connectionSettings.SasMinutes * 60 * 1000) - 10, 0);

            return connAck;
        }

            public async Task<IMqttBaseClient> CreateDpsClientAsync(string connectionSettingsString, CancellationToken cancellationToken = default)
        {
            MqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient() as MqttClient;
            var cs = new ConnectionSettings(connectionSettingsString);
            if (cs.Auth == AuthType.Sas)
            {
                var resource = $"{cs.IdScope}/registrations/{cs.DeviceId}";
                var username = $"{resource}/api-version=2019-03-31";
                var password = SasAuth.CreateSasToken(resource, cs.SharedAccessKey, 5);
                var options = new MqttClientOptionsBuilder()
                    .WithClientId(cs.DeviceId)
                    .WithTcpServer("global.azure-devices-provisioning.net", 8883)
                    .WithCredentials(username, password)
                    .WithTls(new MqttClientOptionsBuilderTlsParameters
                    {
                        UseTls = true,
                        CertificateValidationHandler = (x) => { return true; },
                        SslProtocol = SslProtocols.Tls12
                    })
                .Build();
                await mqtt.ConnectAsync(options, cancellationToken);
            }
            else if (cs.Auth == AuthType.X509)
            {
                var cert = ClientCertificateLocator.Load(cs.X509Key);
                string registrationId = X509CommonNameParser.GetCNFromCertSubject(cert.Subject);
                var resource = $"{cs.IdScope}/registrations/{registrationId}";
                var username = $"{resource}/api-version=2019-03-31";

                var options = new MqttClientOptionsBuilder()
                    .WithClientId(registrationId)
                    .WithTcpServer("global.azure-devices-provisioning.net", 8883)
                    .WithCredentials(username)
                    .WithTls(new MqttClientOptionsBuilderTlsParameters
                    {
                        UseTls = true,
                        CertificateValidationHandler = (x) => { return true; },
                        SslProtocol = SslProtocols.Tls12,
                        Certificates = new List<X509Certificate> { cert }
                    })
                    .Build();

                await mqtt.ConnectAsync(options, cancellationToken);
            }
            return new MqttNetClient(mqtt) { ConnectionSettings = cs };
        }

        public async Task<IMqttBaseClient> CreateBasicClientAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            IMqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient();
            var connack = await mqtt.ConnectAsync(new MqttClientOptionsBuilder()
                .WithBasicAuth(cs)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(cs.KeepAliveInSeconds))
                .Build(), cancellationToken);
            if (connack.ResultCode != MqttClientConnectResultCode.Success)
            {
                throw new ApplicationException(connack.ReasonString);
            }
            return new MqttNetClient(mqtt) { ConnectionSettings =cs };
        }

        public async Task<IMqttBaseClient> CreateAwsClientAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            MqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient() as MqttClient;
            var connAck = await mqtt.ConnectAsync(new MqttClientOptionsBuilder()
                .WithAwsX509Credentials(cs)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(cs.KeepAliveInSeconds))
                .Build(), cancellationToken);
            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }
            return new MqttNetClient(mqtt) { ConnectionSettings = cs };
        }
    }
}
