using pnp_memmon;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<DeviceRunner>();
    })
    .ConfigureHostOptions(configureOptions => configureOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore)
    .Build();

await host.RunAsync();
