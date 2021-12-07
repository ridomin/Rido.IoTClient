using pnp_memmon;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<DeviceRunner>();
    })
    .Build();

await host.RunAsync();
