// See https://aka.ms/new-console-template for more information
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Implementations;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub;
using Rido.IoTClient.AzIoTHub.TopicBindings;

Console.WriteLine("Hello, World!");

var cs = ConnectionSettings.FromConnectionString(Environment.GetEnvironmentVariable("cs"));


IMqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger())
                            .CreateMqttClient(new MqttClientAdapterFactory());

var connAck = await mqtt.ConnectAsync(
    new MqttClientOptionsBuilder()
        .WithAzureIoTHubCredentials(cs)
        .Build());

//var deviceInfo = new ReadOnlyProperty<string>(mqtt, "serialNumber", "deviceInfo");

var v = await new UpdateTwinBinder(mqtt).ReportPropertyAsync(new { MyNameIs = "Jonas" });
var twin = await new GetTwinBinder(mqtt).ReadPropertiesDocAsync();
var cmd = new Command<EmptyCommandRequest, EmptyCommandResponse>(mqtt, "myCommand");

cmd.OnCmdDelegate = async m =>
{
    global::System.Console.WriteLine("Command Invoked");
    return await Task.FromResult(new EmptyCommandResponse() { Status = 200 });
};

Console.WriteLine(twin);
Console.WriteLine(connAck.ResultCode);
Console.WriteLine(connAck.RetainAvailable);

MqttApplicationMessage msg = new MqttApplicationMessageBuilder()
    .WithTopic("pnp/announce/memmon55")
    .WithPayload("modelid:dtmi:my:testl1")
    .WithRetainFlag(true)
    .Build();
var puback = await mqtt.PublishAsync(msg);

Console.WriteLine(puback.ReasonString);




//var connAck = await mqtt.ConnectAsync(new MqttClientOptionsBuilder()
//    .WithTcpServer("f8826e3352314ca98102cfbde8aff20e.s2.eu.hivemq.cloud", 8883).WithTls()
//    .WithClientId("client1")
//    .WithCredentials("client1", "Myclientpwd.000")
//    .Build());