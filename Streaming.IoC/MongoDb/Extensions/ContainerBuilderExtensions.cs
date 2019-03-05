using Autofac;

namespace Streaming.Infrastructure.MongoDb.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder UseMongoDb(this ContainerBuilder containerBuilder, string connectionString, string databaseName)
        {
            Mappings.VideoMappings.Map();
            containerBuilder.RegisterModule(new IoC.MongoDbModule(connectionString, databaseName));
            return containerBuilder;
        }
    }
}
