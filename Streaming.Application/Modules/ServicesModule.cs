using Autofac;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Streaming.Application.Services;

namespace Streaming.Application.Modules
{
    public class ServicesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.Register<IMongoDatabase>(context => new MongoClient("mongodb://localhost:27017")
                   .GetDatabase("streaming"))
                   .SingleInstance();

            builder.Register<IGridFSBucket>(context => new GridFSBucket(context.Resolve<IMongoDatabase>()))
                   .SingleInstance();

            builder.RegisterType<VideoService>()
                   .As<IVideoService>()
                   .InstancePerLifetimeScope();
        }
    }
}
