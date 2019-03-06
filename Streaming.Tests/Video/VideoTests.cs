using Moq;
using NUnit.Framework;
using Streaming.Api.Controllers;
using Streaming.Application.Commands;
using Streaming.Application.Interfaces.Settings;
using Streaming.Application.Query;
using Streaming.Infrastructure.Services;

namespace Streaming.Tests
{
    public class VideoTests
    {
        VideoController videoController;

        [SetUp]
        public void Setup()
        {
            var commandDispatcher = new CommandDispatcher();
            var videoQueries = new VideoQueries();

            var secretServerKey = new Mock<ISecretServerKey>();
            secretServerKey.Setup(x => x.SecretServerKey).Returns("test");

            var messageSignerService = new SHA256MessageSignerService(secretServerKey.Object);

            var commandBus = new Mock<ICommandBus>();
            commandBus.Setup(x => x.Push(It.IsAny<ICommand>())).Callback(() => { });

            videoController = new VideoController(new CommandDispatcher(), new VideoQueries(), messageSignerService);
            videoController.ControllerContext
        }

        [Test]
        public void Test1()
        {
        }
    }
}