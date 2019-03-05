using Autofac;
using Streaming.Application.Query;
using System.Reflection;

namespace Streaming.IoC
{
    public class QueryModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var assembly = typeof(IVideoQueries).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(assembly)
                   .InNamespaceOf<IVideoQueries>()
                   .AsImplementedInterfaces();
        }
    }
}
