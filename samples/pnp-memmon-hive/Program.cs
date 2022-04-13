using pnp_memmon_hive;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<DeviceRunner>();
    })
    .Build();

await host.RunAsync();
