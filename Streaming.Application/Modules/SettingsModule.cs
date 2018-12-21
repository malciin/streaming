using Autofac;
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

            builder.RegisterAssemblyTypes(currentAssembly)
                   .InNamespaceOf<IKeysSettings>();
        }
    }
}
