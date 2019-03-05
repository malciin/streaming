using Autofac;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using Streaming.Application.Services;
using Streaming.Domain.Models;

namespace Streaming.IoC
{
    public class ServicesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

			builder.Register<IMongoDatabase>(context => {
                var connectionString = context.Resolve<IDatabaseSettings>().ConnectionString;
                return new MongoClient(connectionString)
                       .GetDatabase("streaming");
            }).SingleInstance();

			builder.Register(context =>
			{
				return context.Resolve<IMongoDatabase>().GetCollection<Video>("Videos");
			}).As<IMongoCollection<Video>>().InstancePerLifetimeScope();

            builder.Register<IGridFSBucket>(context => new GridFSBucket(context.Resolve<IMongoDatabase>()))
                   .SingleInstance();

            builder.RegisterType<FFmpegProcessVideoService>()
                   .As<IProcessVideoService>()
                   .SingleInstance();

            builder.RegisterType<LocalStorageVideoBlobService>()
                   .As<IVideoBlobService>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<ThumbnailService>()
                   .As<IThumbnailService>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<AzureBlobClient>()
                   .As<IAzureBlobClient>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<LoggerService>()
                   .As<ILoggerService>()
                   .SingleInstance();

            builder.RegisterType<Auth0ManagementApiTokenAccessor>()
                   .As<IAuth0ManagementApiTokenAccessor>()
                   .SingleInstance();

            builder.RegisterType<SHA256MessageSignerService>()
                   .As<IMessageSignerService>()
                   .SingleInstance();
        }
    }
}
