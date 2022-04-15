using pnp_memmon_hub;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Device>();
    })
    .Build();

await host.RunAsync();
