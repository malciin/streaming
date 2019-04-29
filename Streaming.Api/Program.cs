using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Streaming.Api
{
    public class Program
    {

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((ctx, config) =>
                {
                    var envName = ctx.HostingEnvironment.EnvironmentName;
                    config.AddJsonFile("Configuration.json", optional: false, reloadOnChange: false);
                    config.AddJsonFile($"Configuration.{envName}.json", optional: true, reloadOnChange: false);
                })
                .UseKestrel((ctx, options) =>
                {
                    var defaultPort = int.Parse(ctx.Configuration["Hosting:HttpPort"]);
                    options.ListenLocalhost(defaultPort);
                })
                .UseStartup<Startup>();
    }
}
