using Autofac;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Streaming.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Services
{
    public class _ServicesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.Register<IMongoDatabase>(context => new MongoClient("mongodb://localhost:27017").GetDatabase("streaming")).SingleInstance();
            builder.Register<IGridFSBucket>(context => new GridFSBucket(context.Resolve<IMongoDatabase>())).SingleInstance();
            builder.RegisterType<VideoService>().As<IVideoService>().InstancePerLifetimeScope();
        }
    }
}
