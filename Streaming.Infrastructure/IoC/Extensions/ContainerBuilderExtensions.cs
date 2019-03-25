using Autofac;

namespace Streaming.Infrastructure.IoC.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder UseDefaultModules(this ContainerBuilder builder)
        {
            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<QueryModule>();
            builder.RegisterModule<EventModule>();
            builder.RegisterModule<ServicesModule>();
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<MappingModule>();
            builder.RegisterModule<StrategiesModule>();
            return builder;
        }
    }
}
