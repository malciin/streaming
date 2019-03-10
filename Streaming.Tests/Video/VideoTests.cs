using Autofac;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using Streaming.Api.Controllers;
using Streaming.Application.Commands;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using Streaming.Application.Mappings;
using Streaming.Application.Query;
using Streaming.Domain.Models;
using Streaming.Infrastructure.Services;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Streaming.Tests
{
    public class VideoTests
    {
        VideoController videoController;

        public ContainerBuilder GetMockedContainerBuilder()
        {
            var builder = new ContainerBuilder();
            //builder.RegisterModule<Infrastructure.IoC.CommandModule>();

            var secretServerKey = new Mock<ISecretServerKey>();
            secretServerKey.Setup(x => x.SecretServerKey).Returns("test");
            builder.Register(x => secretServerKey.Object).AsImplementedInterfaces();

            var assembly = typeof(CommandDispatcher).GetTypeInfo().Assembly;
            builder.RegisterType<CommandDispatcher>()
                   .As<ICommandDispatcher>()
                   .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(assembly)
                   .AsClosedTypesOf(typeof(ICommandHandler<>))
                   .InstancePerLifetimeScope();

            var commandBus = new Mock<ICommandBus>();
            commandBus.Setup(x => x.Push(It.IsAny<ICommand>())).Callback(() => { });
            builder.Register(x => commandBus.Object).AsImplementedInterfaces();

            var directoriesSettings = new Mock<IDirectoriesSettings>();
            builder.Register(x => directoriesSettings.Object).AsImplementedInterfaces();

            var thumbnailService = new Mock<IThumbnailService>();
            thumbnailService.Setup(x => x.GetThumbnailUrl(It.IsAny<Guid>())).Returns("https://testurl.com/thumb.jpg");
            thumbnailService.Setup(x => x.GetPlaceholderThumbnailUrl()).Returns("https://placeholder-testurl.com/thumb.jpg");
            builder.Register(x => thumbnailService.Object).AsImplementedInterfaces();

            var videoBlobService = new Mock<IVideoBlobService>();
            builder.Register(x => videoBlobService.Object).AsImplementedInterfaces();

            var videoCollection = new Mock<IMongoCollection<Video>>();
            videoCollection.Setup(x => x.InsertOneAsync(It.IsAny<Video>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
                .Returns((Video video, InsertOneOptions options, CancellationToken cancellationToken) =>
                {

                    return Task.FromResult(0);
                });
            builder.Register(x => videoCollection.Object).AsImplementedInterfaces();

            return builder;
        }

        [SetUp]
        public void Setup()
        {
            var componentContext = GetMockedContainerBuilder().Build();

            var commandDispatcher = new CommandDispatcher(componentContext);
            var videoQueries = new VideoQueries(new VideoMappings(
                componentContext.Resolve<IThumbnailService>()), 
                componentContext.Resolve<IMongoCollection<Video>>(), 
                componentContext.Resolve<IDirectoriesSettings>(),
                componentContext.Resolve<IVideoBlobService>());

            var messageSignerService = new SHA256MessageSignerService(componentContext.Resolve<ISecretServerKey>());

            videoController = new VideoController(
                commandDispatcher, 
                videoQueries, messageSignerService);
        }
    }
}