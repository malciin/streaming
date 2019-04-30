using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Streaming.Api;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Models;
using Streaming.Domain.Models;

namespace Streaming.Tests.EndToEnd
{
    public class TestWebhost : ITestWebhost
    {
        private readonly ITestDatabase database;
        private IWebHost hostServer;
        private readonly StartupEvents startupEvents;
        private readonly RequestedTestClaims requestedTestClaimsObj;
        private bool useTestUser = false;

        public bool WebhostStarted { get; set; }

        public Uri ApiUri { get; set; }

        public TestWebhost()
        {
            WebhostStarted = false;
            ApiUri = null;
            startupEvents = new StartupEvents();
            requestedTestClaimsObj = new RequestedTestClaims();
            database = new DockerTestDatabase();
        }

        public ITestWebhost ConfigureAutofacServices(Action<ContainerBuilder> services)
        {
            startupEvents.ServicesConfigurationCallbacks.Add(services);
            return this;
        }

        public ITestWebhost ConfigureAppBegining(Action<IApplicationBuilder> app)
        {
            startupEvents.AppConfigurationBeginingCallbacks.Add(app);
            return this;
        }

        public ITestWebhost ConfigureAppAfterAuthentication(Action<IApplicationBuilder> app)
        {
            startupEvents.AppConfigurationAfterAuthenticationCallbacks.Add(app);
            return this;
        }

        public ITestWebhost Start()
        {
            var databaseConnectionString = database.Start();
            hostServer = WebHost.CreateDefaultBuilder(null)
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("Configuration.json", optional: false, reloadOnChange: false);
                    config.AddInMemoryCollection(new List<KeyValuePair<string, string>> {
                        new KeyValuePair<string, string>("Database:ConnectionString", databaseConnectionString)
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IStartupEvents>(factory => startupEvents);
                    services.AddSingleton<RequestedTestClaims>(factory => requestedTestClaimsObj);
                })
                .UseStartup<Startup>()
                .Build();

            hostServer.Start();
            WebhostStarted = true;
            ApiUri = new Uri(hostServer.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First(x => x.StartsWith("http://")));

            return this;
        }

        public ITestWebhost Stop()
        {
            hostServer.StopAsync().GetAwaiter().GetResult();
            ApiUri = null;
            WebhostStarted = false;
            return this;
        }

        public ITestWebhost ConfigureTestUser(params string[] claims)
        {
            this.requestedTestClaimsObj.RequestedClaims = claims;
            if (!useTestUser)
            {
                useTestUser = true;
                ConfigureAppAfterAuthentication(app =>
                {
                    app.UseMiddleware<AuthenticationTestMiddleware>();
                });
            }
            return this;
        }

        public ITestWebhost SeedDatabase(IEnumerable<Video> objects)
        {
            ConfigureAppBegining(app =>
            {
                var repo = app.ApplicationServices.GetService<IVideoRepository>();
                foreach(var video in objects)
                {
                    repo.AddAsync(video).GetAwaiter().GetResult();
                }
                repo.CommitAsync().GetAwaiter().GetResult();
            });
            return this;
        }

        private class AuthenticationTestMiddleware
        {
            private readonly string[] requestedClaims;
            private readonly RequestDelegate next;

            public AuthenticationTestMiddleware(RequestDelegate next, RequestedTestClaims requestedTestClaims)
            {
                this.requestedClaims = requestedTestClaims.RequestedClaims;
                this.next = next;
            }

            public async Task Invoke(HttpContext context /* other dependencies */)
            {
                var claims = requestedClaims
                    .Select(claim => new Claim(Claims.ClaimsNamespace, claim))
                    .ToList();
                var claimsIdentity = new ClaimsIdentity(claims);
                context.User.AddIdentity(claimsIdentity);
                await next(context);
            }
        }

        private class RequestedTestClaims
        {
            public string[] RequestedClaims;
        }

        private class StartupEvents : IStartupEvents
        {
            public List<Action<ContainerBuilder>> ServicesConfigurationCallbacks { get; }
            public List<Action<IApplicationBuilder>> AppConfigurationBeginingCallbacks { get; }
            public List<Action<IApplicationBuilder>> AppConfigurationAfterAuthenticationCallbacks { get; }

            public StartupEvents()
            {
                this.ServicesConfigurationCallbacks = new List<Action<ContainerBuilder>>();
                this.AppConfigurationAfterAuthenticationCallbacks = new List<Action<IApplicationBuilder>>();
                this.AppConfigurationBeginingCallbacks = new List<Action<IApplicationBuilder>>();
            }

            private void InvokeCallbacks<T>(T configureObject, List<Action<T>> callbacks)
            {
                foreach (var callback in callbacks)
                    callback.Invoke(configureObject);
            }

            public Action<ContainerBuilder> ConfigureServicesAutofacCallback => builder 
                => InvokeCallbacks(builder, ServicesConfigurationCallbacks);
            public Action<IApplicationBuilder> AppConfigurationBeginingCallback => app
                => InvokeCallbacks(app, AppConfigurationBeginingCallbacks);
            public Action<IApplicationBuilder> AppConfigurationAfterAuthenticationCallback => app
                => InvokeCallbacks(app, AppConfigurationAfterAuthenticationCallbacks);
        }
    }
}
