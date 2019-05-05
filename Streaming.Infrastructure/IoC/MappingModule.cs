using Autofac;
using Streaming.Application;

namespace Streaming.Infrastructure.IoC
{
    class MappingModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<Mapper>()
                   .SingleInstance();
        }
    }
}
