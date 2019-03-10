using Autofac;
using Streaming.Application.Interfaces.Strategies;
using Streaming.Application.Strategies;
using System.Reflection;

namespace Streaming.Infrastructure.IoC
{
    class StrategiesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var assembly = typeof(PathStrategy).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(assembly)
                   .InNamespaceOf<PathStrategy>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
        }
    }
}
