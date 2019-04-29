using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Streaming.Api
{
    public static class BannerPrinter
    {
        public static void Print(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  __                                           ");
            Console.WriteLine(" (_ _|_ ._ _   _. ._ _  o ._   _    ._   _ _|_ ");
            Console.WriteLine(" __) |_ | (/_ (_| | | | | | | (_| o | | (/_ |_ ");
            Console.WriteLine("      ver. Development        __|              ");
            Console.WriteLine();
            Console.ForegroundColor = defaultColor;
            Console.WriteLine($"Application name: {hostingEnvironment.ApplicationName}");
            Console.WriteLine($"Running in environment: {hostingEnvironment.EnvironmentName}");
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
