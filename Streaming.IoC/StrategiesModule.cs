using Autofac;
using Streaming.Application.Strategies;
using System.Reflection;

namespace Streaming.IoC
{
    public class StrategiesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var assembly = typeof(IPathStrategy).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(assembly)
                   .InNamespaceOf<IPathStrategy>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
        }
    }
}
