using Autofac;
using MongoDB.Driver;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Domain.Models;

namespace Streaming.Infrastructure.MongoDb.IoC
{
    class MongoDbModule : Autofac.Module
    {
        private readonly string connectionString;
        private readonly string databaseName;

        public MongoDbModule(string connectionString, string databaseName)
        {
            this.connectionString = connectionString;
            this.databaseName = databaseName;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register<IMongoDatabase>(context => new MongoClient(connectionString).GetDatabase(databaseName))
                   .SingleInstance();

            builder.Register(context => context.Resolve<IMongoDatabase>().GetCollection<Video>("Videos"))
                   .As<IMongoCollection<Video>>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<Repositories.VideoRepository>()
                   .As<IVideoRepository>()
                   .InstancePerLifetimeScope();
        }
    }
}
