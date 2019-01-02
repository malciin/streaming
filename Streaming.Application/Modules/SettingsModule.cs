using Autofac;
using Microsoft.Extensions.Configuration;
using Streaming.Application.Settings;
using System.Reflection;

namespace Streaming.Application.Modules
{
    public class SettingsModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var currentAssembly = typeof(SettingsModule).GetTypeInfo().Assembly;

            builder.Register(context => (IConfigurationRoot)context.Resolve<IConfiguration>())
                   .As<IConfigurationRoot>();

            builder.RegisterAssemblyTypes(currentAssembly)
                   .InNamespaceOf<IKeysSettings>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
        }
    }
}
