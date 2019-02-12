using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Streaming.Api
{
    public class Program
    {
        private static IConfigurationRoot configurationRoot;

        public static void Main(string[] args)
        {
            configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Configuration.json", optional: false, reloadOnChange: false)
                .Build();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://localhost:{configurationRoot["Hosting:HttpPort"]}")
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("Configuration.json", optional: false, reloadOnChange: false);
                    config.AddJsonFile($"Configuration.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: false);
                })
                .UseStartup<Startup>();
    }
}
