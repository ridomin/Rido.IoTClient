using MQTTnet.Client;

namespace Rido.IoTClient.AzIoTHub
{
    public static class MqttNetExtensions
    {
        public static MqttClientOptionsBuilder WithAzureIoTHubCredentials(this MqttClientOptionsBuilder builder, ConnectionSettings cs) =>
            WithAzureIoTHubCredentials(builder, cs.HostName, cs.DeviceId, cs.SharedAccessKey, cs.ModelId, cs.SasMinutes);

        public static MqttClientOptionsBuilder WithAzureIoTHubCredentials(this MqttClientOptionsBuilder builder, string hostName, string deviceId, string sasKey, string modelId, int sasMinutes)
        {
            (string username, string password) = SasAuth.GenerateHubSasCredentials(hostName, deviceId, sasKey, modelId, sasMinutes);
            builder
                .WithTcpServer(hostName, 8883)
                .WithTls()
                .WithClientId(deviceId)
                .WithCredentials(username, password);
            return builder;
        }
    }
}
