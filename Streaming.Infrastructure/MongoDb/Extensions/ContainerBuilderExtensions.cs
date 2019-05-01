using Autofac;
using Streaming.Infrastructure.MongoDb.Mappings;
using System.Linq;

namespace Streaming.Infrastructure.MongoDb.Extensions
{
    public static class ContainerBuilderExtensions
    {
        private static void RunAllMongoDbMappingMethods()
        {
            var mongoDbMappingsMethods = typeof(_MappingsMarker).Assembly.GetTypes()
                .Where(x => x.IsInNamespaceOf<_MappingsMarker>())
                .Where(x => x.GetMethods().Any(y => y.Name == "Map"))
                .Select(x => x.GetMethod("Map"));
            foreach (var mapMethod in mongoDbMappingsMethods)
                mapMethod?.Invoke(null, new object[] { });
        }

        public static ContainerBuilder UseMongoDb(this ContainerBuilder containerBuilder, string connectionString)
        {
            RunAllMongoDbMappingMethods();

            containerBuilder.RegisterModule(new IoC.MongoDbModule(connectionString));
            return containerBuilder;
        }
    }
}
