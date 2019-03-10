using Autofac;

namespace Streaming.Infrastructure.IoC.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder UseDefaultModules(this ContainerBuilder builder)
        {
            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<ServicesModule>();
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<QueryModule>();
            builder.RegisterModule<MappingModule>();
            builder.RegisterModule<StrategiesModule>();
            return builder;
        }
    }
}
