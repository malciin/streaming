﻿using Autofac;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
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

            builder.Register<IClientSessionHandle>(context => new MongoClient(connectionString).StartSession())
                   .InstancePerLifetimeScope();

            builder.Register<IMongoDatabase>(context => new MongoClient(connectionString).GetDatabase(databaseName))
                   .SingleInstance();

            builder.Register<IGridFSBucket>(context => new GridFSBucket(context.Resolve<IMongoDatabase>()))
                   .SingleInstance();

            builder.Register(context => context.Resolve<IMongoDatabase>().GetCollection<Video>("Videos"))
                   .As<IMongoCollection<Video>>()
                   .InstancePerLifetimeScope();

            builder.Register(context => context.Resolve<IMongoDatabase>().GetCollection<LiveStream>("LiveStream"))
                   .As<IMongoCollection<LiveStream>>()
                   .InstancePerLifetimeScope();

			builder.RegisterType<Repositories.VideoRepository>()
				   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
        }
    }
}
