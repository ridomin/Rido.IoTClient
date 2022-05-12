using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Serialization;
using System.Text.Json;
using static Rido.HubProxy.Imemmon;

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

var dtc = DigitalTwinClient.CreateFromConnectionString(app.Configuration.GetConnectionString("hub"));

app.MapGet("/pnp/{did}/props/started", async (string did) =>
{
    var twin = await dtc.GetDigitalTwinAsync<BasicDigitalTwin>(did);
    if (twin.Body.CustomProperties.TryGetValue("started", out object? propValue))
    {
        if (DateTime.TryParse(propValue.ToString(), out DateTime started))
        {
            return Results.Ok(started);
        }
        else
        {
            return Results.Problem("Property 'started' is not a DateTime", propValue.ToString(), 405);
        }
    }
    else
    {
        return Results.NotFound();
    }
    
}).WithName("readProperty_started");


app.MapGet("/pnp/{did}/props/interval", async (string did) =>
{
    var twin = await dtc.GetDigitalTwinAsync<BasicDigitalTwin>(did);
    if (twin.Body.CustomProperties.TryGetValue("interval", out object? propValue))
    {
        if (int.TryParse(propValue.ToString(), out int interval))
        {
            return Results.Ok(interval);
        }
        else
        {
            return Results.Problem("Property 'interval' is not an int", propValue.ToString(), 405);
        }
    }
    else
    {
        return Results.NotFound();
    }

}).WithName("readProperty_interval");

app.MapGet("/pnp/{did}/props/enabled", async (string did) =>
{
    var twin = await dtc.GetDigitalTwinAsync<BasicDigitalTwin>(did);
    if (twin.Body.CustomProperties.TryGetValue("enabled", out object? propValue))
    {
        if (bool.TryParse(propValue.ToString(), out Boolean enabled))
        {
            return Results.Ok(enabled);
        }
        else
        {
            return Results.Problem("Property 'enabled' is not an int", propValue.ToString(), 405);
        }
    }
    else
    {
        return Results.NotFound();
    }

}).WithName("readProperty_enabled");



app.MapPost("/pnp/{did}/props/interval", async (string did, [FromBody]int propVal) =>
{
    var updOp = new UpdateOperationsUtility();
    updOp.AppendAddPropertyOp("/interval", propVal);
    var res = await dtc.UpdateDigitalTwinAsync(did, updOp.Serialize());
    res.Response.EnsureSuccessStatusCode();
    return Results.StatusCode(((int)res.Response.StatusCode));
    
}).WithName("updateProperty_interval");


app.MapPost("/pnp/{did}/props/enabled", async (string did, [FromBody] bool propVal) =>
{
    var updOp = new UpdateOperationsUtility();
    updOp.AppendAddPropertyOp("/enabled", propVal);
    var res = await dtc.UpdateDigitalTwinAsync(did, updOp.Serialize());
    res.Response.EnsureSuccessStatusCode();
    return Results.StatusCode(((int)res.Response.StatusCode));

}).WithName("updateProperty_enabled");

app.MapPost("/pnp/{did}/commands/getRuntimeStats", async (string did, [FromBody] DiagnosticsMode diagMode) =>
{
    var resp = await dtc.InvokeCommandAsync(did, "getRuntimeStats", JsonSerializer.Serialize(diagMode),
        new DigitalTwinInvokeCommandRequestOptions { ConnectTimeoutInSeconds = 3, ResponseTimeoutInSeconds = 5 });
    resp.Response.EnsureSuccessStatusCode();
    var result = JsonSerializer.Deserialize<Cmd_getRuntimeStats_Response>(resp.Body.Payload);
    return Results.Ok(result);
    
}).WithName("command_getRuntimeStats");

app.Run();
