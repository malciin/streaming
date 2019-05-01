using System.Linq;
using Autofac;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Domain.Models;
using Streaming.Infrastructure.MongoDb.Repositories;

namespace Streaming.Infrastructure.MongoDb.IoC
{
    class MongoDbModule : Autofac.Module
    {
        private readonly string connectionString;

        public MongoDbModule(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register<IClientSessionHandle>(context => new MongoClient(connectionString).StartSession())
                   .InstancePerLifetimeScope();

            builder.Register<IMongoDatabase>(context => new MongoClient(connectionString).GetDatabase(MongoDbNames.DatabaseName))
                   .SingleInstance();

            builder.Register<IGridFSBucket>(context => new GridFSBucket(context.Resolve<IMongoDatabase>()))
                   .SingleInstance();

            builder.RegisterType<UnitOfWork>()
                   .As<IUnitOfWork>()
                   .InstancePerLifetimeScope();

            builder.Register(context => context.Resolve<IMongoDatabase>().GetCollection<Video>(MongoDbNames.CollectionNames[typeof(Video)]))
                   .As<IMongoCollection<Video>>()
                   .InstancePerLifetimeScope();

            builder.Register(context => context.Resolve<IMongoDatabase>().GetCollection<LiveStream>(MongoDbNames.CollectionNames[typeof(LiveStream)]))
                   .As<IMongoCollection<LiveStream>>()
                   .InstancePerLifetimeScope();

            var assembly = typeof(IRepositoryMarker).Assembly;
            builder.RegisterAssemblyTypes(assembly)
                .InNamespaceOf<IRepositoryMarker>()
                .Where(x => x.IsClass && x.GetInterfaces().Contains(typeof(IRepositoryMarker)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
