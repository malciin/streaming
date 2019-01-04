using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
