using System;
using Autofac;
using Microsoft.AspNetCore.Builder;

namespace Streaming.Api
{
    public interface IStartupEvents
    {
        Action<ContainerBuilder> ConfigureServicesAutofacCallback { get; }
        Action<IApplicationBuilder> AppConfigurationBeginingCallback { get; }
        Action<IApplicationBuilder> AppConfigurationAfterAuthenticationCallback { get; }
    }

    internal class NoneStartupEvents : IStartupEvents
    {
        public Action<ContainerBuilder> ConfigureServicesAutofacCallback => (_) => { };
        public Action<IApplicationBuilder> AppConfigurationBeginingCallback => (_) => { };
        public Action<IApplicationBuilder> AppConfigurationAfterAuthenticationCallback => (_) => { };
    }
}
