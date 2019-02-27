using Autofac;
using Streaming.Application.Strategies;
using System.Reflection;

namespace Streaming.Application.Modules
{
    public class StrategiesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var currentAssembly = typeof(StrategiesModule).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(currentAssembly)
                   .InNamespaceOf<IPathStrategy>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
        }
    }
}
