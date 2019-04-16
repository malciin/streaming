using Auth0.ManagementApi;
using Autofac;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using Streaming.Application.Models;
using Streaming.Infrastructure.Services;


namespace Streaming.Infrastructure.IoC
{
    public class ServicesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<FFmpegProcessVideoService>()
                   .As<IProcessVideoService>()
                   .As<IVideoFileInfoService>()
                   .SingleInstance();

            builder.RegisterType<VideoFilesLocalService>()
                   .As<IVideoFilesService>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<ThumbnailLocalService>()
                   .As<IThumbnailService>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<AzureBlobClient>()
                   .As<IAzureBlobClient>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<LoggerService>()
                   .As<ILoggerService>()
                   .SingleInstance();

            builder.RegisterType<SHA256MessageSignerService>()
                   .As<IMessageSignerService>()
                   .SingleInstance();

            builder.RegisterType<LiveStreamManager>()
                   .As<ILiveStreamManager>()
                   .SingleInstance();

            builder.RegisterType<Auth0ClientWrapper>()
                   .As<IAuth0Client>()
                   .SingleInstance();
        }
    }
}
