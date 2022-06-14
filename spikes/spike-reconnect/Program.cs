using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using Rido.Mqtt.HubClient;
using Rido.Mqtt.MqttNet4Adapter;
using Rido.MqttCore;
using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;

namespace spike_reconnect
{
    internal class Program
    {
        const string mk = "MDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDA=";
        internal static string ComputeDeviceKey(string masterKey, string deviceId) =>
          Convert.ToBase64String(new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(masterKey)).ComputeHash(System.Text.Encoding.UTF8.GetBytes(deviceId)));

        static Stopwatch clock = Stopwatch.StartNew();
        static ConnectionSettings cs;
        static ManagedMqttClient mqttClient;
        static async Task Main(string[] args)
        {

            cs = new ConnectionSettings()
            {
                HostName = "tests.azure-devices.net",
                DeviceId = "d8",
                SharedAccessKey = mk,
                SasMinutes = 2
            };

            //IMqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient();
            //var ops = new MqttClientOptionsBuilder().WithAzureIoTHubCredentials(cs).Build();
            //var connAck = await mqtt.ConnectAsync(ops);
            //Console.WriteLine(connAck.AssignedClientIdentifier);

            // Setup and start a managed MQTT client.
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithAzureIoTHubCredentials(cs)
                    .Build())
                .Build();

            mqttClient = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateManagedMqttClient() as ManagedMqttClient;
            //await mqttClient.SubscribeAsync("my/topic");
            mqttClient.ConnectionStateChangedAsync += MqttClient_ConnectionStateChangedAsync;
            mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
            await mqttClient.StartAsync(options);

            mqttClient.ConnectedAsync += async ea =>
            {
                Console.WriteLine("Connected");
                Console.WriteLine(DateTime.Now);
                var hub = new HubMqttClient(new MqttNetClient(mqttClient.InternalClient));
                while (true)
                {
                    //await mqttClient.InternalClient.PingAsync();
                    //await mqtt.PingAsync();

                    await hub.SendTelemetryAsync(new { Environment.WorkingSet });
                    Console.WriteLine(clock.Elapsed.TotalSeconds);
                    await Task.Delay(60000);
                }
            };
            Console.ReadLine();
        }

        private static async Task MqttClient_ConnectionStateChangedAsync(EventArgs arg)
        {
            await Task.Yield();
            Console.WriteLine("Connection Changed");
        }

        private static async Task MqttClient_DisconnectedAsync(EventArgs arg)
        {
            Console.WriteLine("Disconnected");
            var options = new MqttClientOptionsBuilder()
                    .WithAzureIoTHubCredentials(cs)
                    .Build();

            mqttClient.Options.ClientOptions = options;
            Console.WriteLine("Reconnecting: " +  clock.Elapsed.TotalMinutes);
            await Task.Yield();
         
        }

        //private static async Task Mqtt_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        //{
        //    Console.WriteLine(clock.Elapsed.TotalSeconds);
        //    await Task.Yield();
        //    Console.WriteLine(arg.ReasonString);
        //}
    }
}
