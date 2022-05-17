using Microsoft.AspNetCore.Mvc;
using Rido.Mqtt.MqttNet3Adapter;
using Rido.MqttCore;
using Rido.MqttCore.PnP;
using System.Text.Json;
using static Rido.BrokerProxy.Imemmon;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var mqtt = await new MqttNetClientConnectionFactory().CreateBasicClientAsync(new ConnectionSettings(app.Configuration.GetConnectionString("broker")));

Dictionary<string, Dictionary<string, object>> devices = new Dictionary<string, Dictionary<string, object>>();
TaskCompletionSource<Cmd_getRuntimeStats_Response> tcs = new TaskCompletionSource<Cmd_getRuntimeStats_Response>();

mqtt.OnMessage += async m =>
{
    app.Logger.LogInformation(m.Topic);
    var segments = m.Topic.Split('/');
    var did = segments[1];
    if (!devices.ContainsKey(did))
    {
        devices.Add(did, new Dictionary<string, object>());
    }
    if (segments[2] != null && segments[2] == "props")
    {
        if (segments.Length == 4)
        {
            var props = devices[did];
            if (props.ContainsKey(segments[3]))
            {
                props[segments[3]] = m.Payload;
            }
            else
            {
                props.Add(segments[3], m.Payload);
            }
        }
    }

    if (segments[2] != null && segments[2] == "commands")
    {
        if (segments.Length == 6 && segments[3]=="getRuntimeStats" && segments[4] == "resp")
        {
            if (segments[5]=="200")
            {
                tcs.SetResult(JsonSerializer.Deserialize<Cmd_getRuntimeStats_Response>(m.Payload));
            }
            else
            {
                tcs.SetException(new ApplicationException(m.Payload));
            }
        }

    }

};
await mqtt.SubscribeAsync("pnp/+/props/#");
await mqtt.SubscribeAsync("pnp/+/commands/#");


app.MapGet("/pnp/{did}/props/started", async (string did) =>
{
    if (devices.ContainsKey(did) && devices[did].ContainsKey("started"))
    {
        var result = JsonSerializer.Deserialize<DateTime>(devices[did]["started"].ToString());
        return Results.Ok(result);
        
    }
    return Results.NotFound();
}).WithName("readProperty_started").WithTags(new string[] { "pnp" });

app.MapGet("/pnp/{did}/props/interval", async (string did) =>
{
    if (devices.ContainsKey(did) && devices[did].ContainsKey("interval"))
    {
        var result = JsonSerializer.Deserialize<PropertyAck<int>>(devices[did]["interval"].ToString());
        return Results.Ok(result);
    }
    return Results.NotFound();
}).WithName("readProperty_interval").WithTags(new string[] { "pnp" });

app.MapPost("/pnp/{did}/props/interval", async (string did, [FromBody] int propVal) =>
{
    var pubAck = await mqtt.PublishAsync($"pnp/{did}/props/interval/set", propVal);
    return Results.Ok(pubAck);

}).WithName("updateProperty_interval").WithTags(new string[] { "hub" });

app.MapGet("/pnp/{did}/props/enabled", async (string did) =>
{
    if (devices.ContainsKey(did) && devices[did].ContainsKey("enabled"))
    {
        var result = JsonSerializer.Deserialize<PropertyAck<bool>>(devices[did]["enabled"].ToString());
        return Results.Ok(result);
    }
    return Results.NotFound();
}).WithName("readProperty_enabled").WithTags(new string[] { "pnp" });

app.MapPost("/pnp/{did}/props/enabled", async (string did, [FromBody] bool propVal) =>
{
    var pubAck = await mqtt.PublishAsync($"pnp/{did}/props/enabled/set", propVal);
    return Results.Ok(pubAck);

}).WithName("updateProperty_enabled").WithTags(new string[] { "hub" });



app.MapPost("/pnp/{did}/commands/getRuntimeStats", async (string did, [FromBody] DiagnosticsMode diagMode) =>
{
    tcs = new TaskCompletionSource<Cmd_getRuntimeStats_Response>();
    var pubAck = await mqtt.PublishAsync($"pnp/{did}/commands/getRuntimeStats", (int)diagMode);
    return Results.Ok(tcs.Task.Result);
}).WithName("command_getRuntimeStats").WithTags(new string[] { "hub" });


app.Run();

