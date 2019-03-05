using Autofac;
using Streaming.Application.Mappings;
using System.Reflection;

namespace Streaming.IoC
{
    public class MappingModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var assembly = typeof(VideoMappingService).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(assembly)
                   .AssignableTo<IMappingService>()
                   .SingleInstance();
        }
    }
}
