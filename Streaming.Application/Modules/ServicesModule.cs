using Autofac;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Streaming.Domain.Models.Core;

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

			builder.Register(context =>
			{
				return context.Resolve<IMongoDatabase>().GetCollection<Video>("Videos");
			}).As<IMongoCollection<Video>>().InstancePerLifetimeScope();

            builder.Register<IGridFSBucket>(context => new GridFSBucket(context.Resolve<IMongoDatabase>()))
                   .SingleInstance();
        }
    }
}
