using System;
using System.Collections.Generic;
using System.Linq;
using Auth0.ManagementApi.Models;
using Autofac;
using Moq;
using NUnit.Framework;
using Streaming.Application.Commands;
using Streaming.Application.Commands.Live;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Domain.Models;
using Streaming.Infrastructure.IoC;
using Streaming.Tests.Mocks;

namespace Streaming.Tests.Commands.Live
{
    public class StartLiveCommandTests
    {
        private List<LiveStream> liveStreamsInDatabase;
        private List<LiveStream> liveStreams;
        
        private Mock<IPastLiveStreamRepository> liveStreamRepositoryMock;
        private Mock<ILiveStreamManager> liveStreamManagerMock;
        private Mock<IAuth0Client> auth0ClientMock;

        private ICommandDispatcher CommandDispatcher { get; set; }

        [SetUp]
        public void Startup()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<CommandModule>();
            containerBuilder.RegisterModule<ServicesModule>();
            containerBuilder.RegisterModule<MappingModule>();
            
            liveStreamsInDatabase = new List<LiveStream>();
            liveStreamRepositoryMock = LiveStreamRepositoryMock.CreateForData(liveStreamsInDatabase);
            containerBuilder.Register(x => liveStreamRepositoryMock.Object).AsImplementedInterfaces().InstancePerLifetimeScope();

            liveStreams = new List<LiveStream>();
            liveStreamManagerMock = LiveStreamManagerMock.CreateForData(liveStreams);
            containerBuilder.Register(x => liveStreamManagerMock.Object).AsImplementedInterfaces();

            var messageSignerServiceMock = MessageSignerServiceMock.CreateForRandomGuid();
            containerBuilder.Register(x => messageSignerServiceMock.Object).AsImplementedInterfaces();

            auth0ClientMock = new Mock<IAuth0Client>();
            auth0ClientMock.Setup(x => x.GetInfoAsync(It.IsAny<string>())).ReturnsAsync((string id) => new User
            {
                NickName = "malcin",
                UserId = id,
                Email = "email@gmail.com"
            });
            containerBuilder.Register(x => auth0ClientMock.Object).As<IAuth0Client>().SingleInstance();

            var context = containerBuilder.Build();
            CommandDispatcher = context.Resolve<ICommandDispatcher>();
        }

        [Test]
        public void To_Start_LiveStream_Url_Must_Have_Relative_Path_Equal_Live_Otherwise_Throw_Argument_Exception()
        {
            Assert.ThrowsAsync<ArgumentException>(() => CommandDispatcher.HandleAsync(new StartLiveStreamCommand
            {
                App = "some/other/endpoint",
            }));

            Assert.DoesNotThrowAsync(() => CommandDispatcher.HandleAsync(new StartLiveStreamCommand
            {
                App = "live",
                StreamId = Guid.NewGuid(),
                StreamKey = "some key",
                ManifestUri = new Uri("http://localhost:8084/")
            }));
        }

        [Test]
        public void Start_LiveStream_Adding_Works()
        {
            var firstGuid = Guid.NewGuid();
            CommandDispatcher.HandleAsync(new StartLiveStreamCommand
            {
                App = "live",
                StreamId = firstGuid,
                StreamKey = "some key",
                ManifestUri = new Uri("http://localhost:8084")
            }).GetAwaiter().GetResult();

            var addedLivestream = liveStreams.First();
            Assert.AreEqual(firstGuid, addedLivestream.LiveStreamId);
        }
    }
}