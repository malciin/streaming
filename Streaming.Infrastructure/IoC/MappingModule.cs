using Autofac;
using Streaming.Application;

namespace Streaming.Infrastructure.IoC
{
    public class MappingModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<Mapper>()
                   .SingleInstance();
        }
    }
}
