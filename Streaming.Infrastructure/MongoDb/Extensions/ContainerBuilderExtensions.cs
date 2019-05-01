using Autofac;
using Streaming.Infrastructure.MongoDb.Mappings;
using System.Linq;

namespace Streaming.Infrastructure.MongoDb.Extensions
{
    public static class ContainerBuilderExtensions
    {
        private static void runAllMongoDbMappingMethods()
        {
            var mongoDbMappingsMethods = typeof(_MappingsMarker).Assembly.GetTypes()
                .Where(x => x.IsInNamespaceOf<_MappingsMarker>())
                .Where(x => x.GetMethods().Where(y => y.Name == "Map").Count() > 0)
                .Select(x => x.GetMethod("Map"));
            foreach (var mapMethod in mongoDbMappingsMethods)
                mapMethod.Invoke(null, new object[] { });
        }

        public static ContainerBuilder UseMongoDb(this ContainerBuilder containerBuilder, string connectionString)
        {
            runAllMongoDbMappingMethods();

            containerBuilder.RegisterModule(new IoC.MongoDbModule(connectionString));
            return containerBuilder;
        }
    }
}
