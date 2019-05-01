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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;

namespace Streaming.Tests.EndToEnd
{
    public class TestWebhost : ITestWebhost
    {
        private readonly ITestDatabase database;
        private IWebHost hostServer;
        private readonly StartupEvents startupEvents;
        private readonly RequestedTestClaims requestedTestClaimsObj;
        private bool useDatabase = true;

        public IConfigurationRoot Configuration { get; }
        public IServiceProvider Services { get; private set; }
        public bool WebhostStarted { get; set; }

        public Uri ApiUri { get; set; }

        public TestWebhost()
        {
            WebhostStarted = false;
            ApiUri = null;
            startupEvents = new StartupEvents();
            requestedTestClaimsObj = new RequestedTestClaims();
            database = new DockerMongoDbTestDatabase();
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("Configuration.json", optional: false, reloadOnChange: false)
                .Build();
        }

        public ITestWebhost ConfigureAutofacServices(Action<ContainerBuilder> services)
        {
            startupEvents.ServicesConfigurationCallbacks.Add(services);
            return this;
        }

        public ITestWebhost DontUseDatabase()
        {
            this.useDatabase = false;
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
            var databaseConnectionString = String.Empty;
            if (useDatabase)
                databaseConnectionString = database.Start();

            var anyClassMap = BsonClassMap.GetRegisteredClassMaps().FirstOrDefault();
            if (anyClassMap != null)
            {
                var classMapDictionaryField = typeof(BsonClassMap).GetField("__classMaps",
                    BindingFlags.Static | BindingFlags.NonPublic);
                var classMapDictionary = (Dictionary<Type, BsonClassMap>)classMapDictionaryField.GetValue(anyClassMap);
                classMapDictionary.Clear();
            }

            ConfigureAppAfterAuthentication(app =>
            {
                app.UseMiddleware<AuthenticationTestMiddleware>();
            });

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

            this.Services = hostServer.Services;
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
            private readonly RequestDelegate next;

            public AuthenticationTestMiddleware(RequestDelegate next)
            {
                this.next = next;
            }

            public async Task Invoke(HttpContext context, RequestedTestClaims requestedTestClaims)
            {
                var claims = requestedTestClaims.RequestedClaims
                    .Select(claim => new Claim(Claims.ClaimsNamespace, claim))
                    .ToList();
                if (claims.Any())
                {
                    var claimsIdentity = new ClaimsIdentity(claims, "testAuthentication");
                    context.User = new ClaimsPrincipal(claimsIdentity);
                }
                await next(context);
            }
        }

        private class RequestedTestClaims
        {
            public string[] RequestedClaims;

            public RequestedTestClaims()
            {
                RequestedClaims = new string[] { };
            }
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
