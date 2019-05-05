using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        private List<LiveStream> liveStreams;
        private Mock<ILiveStreamRepository> liveStreamRepositoryMock;

        private ICommandDispatcher CommandDispatcher { get; set; }
        
        [SetUp]
        public void Startup()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<CommandModule>();
            containerBuilder.RegisterModule<ServicesModule>();
            
            liveStreams = new List<LiveStream>();
            liveStreamRepositoryMock = LiveStreamRepositoryMock.CreateForData(liveStreams);
            containerBuilder.Register(x => liveStreamRepositoryMock.Object).AsImplementedInterfaces();

            var messageSignerServiceMock = MessageSignerServiceMock.CreateForRandomGuid();
            containerBuilder.Register(x => messageSignerServiceMock.Object).AsImplementedInterfaces();

            var auth0Service = new Mock<IAuth0Client>();
            auth0Service.Setup(x => x.GetInfoAsync(It.IsAny<string>())).ReturnsAsync((string id) => new User
            {
                NickName = "malcin",
                UserId = id,
                Email = "email@gmail.com"
            });
            containerBuilder.Register(x => auth0Service.Object).As<IAuth0Client>().SingleInstance();

            CommandDispatcher = containerBuilder.Build().Resolve<ICommandDispatcher>();
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
    }
}