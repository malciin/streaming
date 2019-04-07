using Autofac;
using Streaming.Application.Strategies;
using System.Reflection;

namespace Streaming.Infrastructure.IoC
{
    public class StrategiesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var assembly = typeof(PathStrategies).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(assembly)
                   .InNamespaceOf<PathStrategies>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
        }
    }
}
