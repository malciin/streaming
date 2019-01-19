using Autofac;
using Streaming.Application.Mappings;
using System.Reflection;

namespace Streaming.Application.Modules
{
    public class MappingModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var currentAssembly = typeof(MappingModule).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(currentAssembly)
                   .AssignableTo<IMappingService>()
                   .SingleInstance();
        }
    }
}
