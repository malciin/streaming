﻿using Autofac;
using Streaming.Application.Interfaces.Services;
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

            builder.RegisterType<Auth0ManagementApiTokenAccessor>()
                   .As<IAuth0ManagementApiTokenAccessor>()
                   .SingleInstance();

            builder.RegisterType<SHA256MessageSignerService>()
                   .As<IMessageSignerService>()
                   .SingleInstance();
        }
    }
}
