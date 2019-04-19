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
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Streaming.Tests.Commands
{
    public class LiveComandsTests : AutofacBasedTestClass
    {
        protected override void SetupRegister(ContainerBuilder builder)
        {
            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<ServicesModule>();

            var liveStreamRepo = new Mock<ILiveStreamRepository>();
            liveStreamRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<LiveStream, bool>>>(), It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(() => new List<LiveStream> { new LiveStream { Title = "some title" } });
            builder.Register(x => liveStreamRepo.Object).AsImplementedInterfaces();

            var messageSigner = new Mock<IMessageSignerService>();
            messageSigner.Setup(x => x.GetMessage(It.IsAny<byte[]>())).Returns(Guid.NewGuid().ToByteArray());
            builder.Register(x => messageSigner.Object).AsImplementedInterfaces();

            var auth0Service = new Mock<IAuth0Client>();
            auth0Service.Setup(x => x.GetInfoAsync(It.IsAny<string>())).ReturnsAsync((string id) => new User
            {
                NickName = "malcin",
                UserId = id,
                Email = "email@gmail.com"
            });
            builder.Register(x => auth0Service.Object).As<IAuth0Client>().SingleInstance();
        }

        [Test]
        public void To_Start_LiveStream_Url_Must_Have_Relative_Path_Equal_Live_Otherwise_Throw_Argument_Exception()
        {
            var dispatcher = Context.Resolve<ICommandDispatcher>();
            Assert.ThrowsAsync<ArgumentException>(() => dispatcher.HandleAsync(new StartLiveStreamCommand
            {
                App = "some/other/endpoint",
            }));

            Assert.DoesNotThrowAsync(() => dispatcher.HandleAsync(new StartLiveStreamCommand
            {
                App = "live",
                StreamId = Guid.NewGuid(),
                StreamKey = "some key",
                ManifestUri = new Uri("http://localhost:8084/")
            }));
        }
    }
}
