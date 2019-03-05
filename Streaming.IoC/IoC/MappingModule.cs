using Autofac;
using Streaming.Application.Mappings;
using System.Reflection;

namespace Streaming.Infrastructure.IoC
{
    class MappingModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var assembly = typeof(VideoMappings).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(assembly)
                   .InNamespaceOf<VideoMappings>()
                   .SingleInstance();
        }
    }
}
