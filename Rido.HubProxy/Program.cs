using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Serialization;
using Microsoft.Extensions.Logging;
using Rido.HubProxy;
using System.Text.Json;

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

//string did = "memmon1";

app.MapGet("/pnp/{did}/props/{prop}", async (string did, string prop) =>
{
    var dtc = DigitalTwinClient.CreateFromConnectionString(app.Configuration.GetConnectionString("hub"));
    var twin = await dtc.GetDigitalTwinAsync<BasicDigitalTwin>(did);
    if (twin.Body.CustomProperties.TryGetValue(prop, out object? propValue))
    {
        return Results.Ok(propValue);
    }
    else
    {
        return Results.NotFound();
    }
    
}).WithName("readProperty");


app.MapPost("/pnp/{did}/props/{prop}", async (string did, string prop, [FromBody]int propVal) =>
{
    var dtc = DigitalTwinClient.CreateFromConnectionString(app.Configuration.GetConnectionString("hub"));
    var updOp = new UpdateOperationsUtility();
    updOp.AppendAddPropertyOp("/" + prop, propVal);
    var res = await dtc.UpdateDigitalTwinAsync(did, updOp.Serialize());
    res.Response.EnsureSuccessStatusCode();
    return Results.StatusCode(((int)res.Response.StatusCode));
    
}).WithName("updateProperty");

//app.MapGet("/pnp/{did}/props/interval", async (string did) =>
//{
//    var dtc = DigitalTwinClient.CreateFromConnectionString(app.Configuration.GetConnectionString("hub"));
//    var twin = await dtc.GetDigitalTwinAsync<BasicDigitalTwin>(did);
//    if (twin.Body.CustomProperties.TryGetValue("interval", out object? rawValue))
//    {
//        if (Int32.TryParse(rawValue.ToString(), out int interval))
//        {
//            return Results.Ok(interval);
//        }
//        else
//        {
//            return Results.Problem("Property started is not int", rawValue.ToString(), 501);
//        }
//    }
//    else
//    {
//        return Results.NotFound();
//    }

//}).WithName("readProperty_interval");




//app.MapGet("/interval", async () => 
//{
//    var rm = RegistryManager.CreateFromConnectionString(app.Configuration.GetConnectionString("hub"));
//    var twin = await rm.GetTwinAsync(did);
//    var intervalProp = TwinProperty<int>.InitFromTwin(twin.ToJson(), "interval", string.Empty, -1);
//    app.Logger.LogInformation("twin:" + twin.ToJson());
//    return intervalProp;
//}).WithName("readProperty_interval");

//app.MapPost("/interval", async (int interval) =>
//{
//    var rm = RegistryManager.CreateFromConnectionString(app.Configuration.GetConnectionString("hub"));
//    var twin = await rm.GetTwinAsync(did);
//    var patch = new
//    {
//        properties = new
//        {
//            desired = new
//            {
//                interval
//            }
//        }
//    };
//    await rm.UpdateTwinAsync(did, JsonSerializer.Serialize(patch), twin.ETag);
//    await Task.Delay(500);
//    twin = await rm.GetTwinAsync(did);
//    var intervalProp = TwinProperty<int>.InitFromTwin(twin.ToJson(), "interval", string.Empty, -1);
//    app.Logger.LogInformation("twin:" + twin.ToJson());
//    return intervalProp;
//}).WithName("writeProperty_interval");




//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateTime.Now.AddDays(index),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");

app.Run();

//internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}