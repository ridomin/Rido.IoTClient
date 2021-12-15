using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace pnp_generic_client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<DeviceRunner>();
                });
    }
}
