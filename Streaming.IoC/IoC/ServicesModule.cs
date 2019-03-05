using Autofac;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Streaming.Application.Interfaces.Services;
using Streaming.Infrastructure.Services;

namespace Streaming.Infrastructure.IoC
{
    class ServicesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

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
