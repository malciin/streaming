using Autofac;
using Streaming.Application.Strategies;

namespace Streaming.Application.Modules
{
    public class StrategiesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<BlobVideoUrlStrategy>()
                   .As<IVideoUrlStrategy>()
                   .SingleInstance();
        }
    }
}
