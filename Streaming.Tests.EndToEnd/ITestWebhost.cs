using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Streaming.Domain.Models;

namespace Streaming.Tests.EndToEnd
{
    public interface ITestWebhost
    {
        bool WebhostStarted { get; }
        
        /// <summary>
        /// It returns api url if WebhostStarted == true
        /// </summary>
        Uri ApiUri { get; }

        /// <summary>
        /// Inject some app configuration at beginning
        /// It can be called multiple times but before ITestWebhost.Start()
        /// </summary>
        /// <param name="app"></param>
        ITestWebhost ConfigureAppBegining(Action<IApplicationBuilder> app);

        /// <summary>
        /// Inject some app configuration after app.UseAuthentication() middleware
        /// It can be called multiple times but before ITestWebhost.Start()
        /// </summary>
        /// <param name="app"></param>
        ITestWebhost ConfigureAppAfterAuthentication(Action<IApplicationBuilder> app);

        /// <summary>
        /// Inject or override some services in Autofac container
        /// </summary>
        /// <param name="services"></param>
        ITestWebhost ConfigureAutofacServices(Action<ContainerBuilder> services);

        /// <summary>
        /// Set default user - to skip Authentication process for testing - Authorization still will work though, based
        /// on given claims. It can be called anywhere and multiple times - the current test user will be overriden
        /// </summary>
        /// <param name="claims"></param>
        ITestWebhost ConfigureTestUser(params string[] claims);

        ITestWebhost SeedDatabase(IEnumerable<Video> objects);

        /// <summary>
        /// Start test webhost - it can be called once
        /// </summary>
        ITestWebhost Start();

        /// <summary>
        /// Stop webhost - it can be called once
        /// </summary>
        /// <returns></returns>
        ITestWebhost Stop();
    }
}
