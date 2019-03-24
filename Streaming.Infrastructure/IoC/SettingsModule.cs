using Autofac;
using Microsoft.Extensions.Configuration;
using Streaming.Application.Settings;
using System.Reflection;

namespace Streaming.Infrastructure.IoC
{
    public class SettingsModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var assembly = typeof(KeysSettings).GetTypeInfo().Assembly;

            builder.Register(context => (IConfigurationRoot)context.Resolve<IConfiguration>())
                   .As<IConfigurationRoot>();

            builder.RegisterAssemblyTypes(assembly)
                   .InNamespaceOf<KeysSettings>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
        }
    }
}
