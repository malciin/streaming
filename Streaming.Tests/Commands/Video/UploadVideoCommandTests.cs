using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth0.ManagementApi.Models;
using Autofac;
using Moq;
using NUnit.Framework;
using Streaming.Application.Commands;
using Streaming.Application.Commands.Video;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using Streaming.Application.Models.DTO.Video;
using Streaming.Application.Models.Enum;
using Streaming.Domain.Enums;
using Streaming.Infrastructure.IoC;
using Streaming.Tests.Extensions;
using Streaming.Tests.Mocks;
using Streaming.Tests.TestExtensions;

namespace Streaming.Tests.Commands.Video
{
    public class UploadVideoCommandTests
    {
        private List<Streaming.Domain.Models.Video> videos;
        private List<VideoCodec> supportedVideoCodecs;
        private VideoFileDetailsDTO videoDetails;

        private Mock<IProcessVideoService> processVideoServiceMock;
        private Mock<IVideoFileInfoService> videoFileInfoServiceMock;
        private Mock<IVideoRepository> videoRepositoryMock;
        private Mock<IAuth0Client> auth0ClientMock;
        private Mock<ICommandBus> commandBusMock;
        private Mock<IMessageSignerService> messageSignerServiceMock;

        private ICommandDispatcher CommandDispatcher { get; set; }

        [SetUp]
        public void Setup()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<CommandModule>();
            containerBuilder.RegisterModule<StrategiesModule>();
            containerBuilder.RegisterUnused(typeof(ILogsDirectorySettings), typeof(ILocalStorageDirectorySettings));
            
            supportedVideoCodecs = new List<VideoCodec> { VideoCodec.h264 };
            processVideoServiceMock = new Mock<IProcessVideoService>();
            processVideoServiceMock.Setup(x => x.SupportedVideoCodecs()).Returns(supportedVideoCodecs);
            containerBuilder.Register(x => processVideoServiceMock.Object).AsImplementedInterfaces();

            videoDetails = new VideoFileDetailsDTO
            {
                Video = {
                    Codec = VideoCodec.h264,
                    Resolution = (1920, 1080),
                    BitrateKbs = 5000
                }, 
                Audio = {
                    Codec = AudioCodec.aac
                },
                Duration = TimeSpan.FromSeconds(5)
            };
            videoFileInfoServiceMock = VideoFileInfoServiceMock.CreateForData(returnedDetails: videoDetails);
            containerBuilder.Register(x => videoFileInfoServiceMock.Object).AsImplementedInterfaces();
            
            videos = new List<Streaming.Domain.Models.Video>();
            videoRepositoryMock = VideoRepositoryMock.CreateForData(videos);
            containerBuilder.Register(x => videoRepositoryMock.Object).AsImplementedInterfaces();
            
            messageSignerServiceMock = new Mock<IMessageSignerService>();
            messageSignerServiceMock.Setup(x => x.GetMessage(It.IsAny<byte[]>())).Returns(Guid.NewGuid().ToByteArray());
            containerBuilder.Register(x => messageSignerServiceMock.Object).AsImplementedInterfaces();
            
            auth0ClientMock = new Mock<IAuth0Client>();
            auth0ClientMock.Setup(x => x.GetInfoAsync(It.IsAny<string>())).ReturnsAsync((string id) => new User
            {
                NickName = "malcin",
                UserId = id,
                Email = "email@gmail.com"
            });
            containerBuilder.Register(x => auth0ClientMock.Object).As<IAuth0Client>().SingleInstance();
            
            commandBusMock = new Mock<ICommandBus>();
            containerBuilder.Register(x => commandBusMock.Object).AsImplementedInterfaces();
            
            CommandDispatcher = containerBuilder.Build().Resolve<ICommandDispatcher>();
        }
        
        [Test]
        public void UploadVideoCommand_Adding_Video_Works()
        {
            CommandDispatcher.HandleAsync(new UploadVideoCommand
            {
                Title = "Title",
                Description = "Desc",
                User = new UserDetailsDTO
                {
                    Email = "email@gmail.com",
                    Nickname = "nick",
                    UserId = "id"
                },
                UploadToken = Convert.ToBase64String(new byte[] { 0x10 })
            }).GetAwaiter().GetResult();

            videoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Streaming.Domain.Models.Video>()), Times.Once);
            Assert.AreEqual(1, videos.Count);
            Assert.AreEqual("Title", videos[0].Title);
            Assert.AreEqual("Desc", videos[0].Description);
            Assert.AreEqual("email@gmail.com", videos[0].Owner.Email);
            Assert.AreEqual("id", videos[0].Owner.UserId);
            Assert.AreEqual("nick", videos[0].Owner.Nickname);
            Assert.AreEqual(VideoState.Fresh, videos[0].State);
        }
        
        [Test]
        public void UploadVideoCommand_Should_Push_ProcessVideoCommand()
        {
            CommandDispatcher.HandleAsync(new UploadVideoCommand
            {
                Title = "Title",
                Description = "Desc",
                User = new UserDetailsDTO
                {
                    Email = "email@gmail.com",
                    Nickname = "nick",
                    UserId = "id"
                },
                UploadToken = Convert.ToBase64String(new byte[] { 0x10 })
            }).GetAwaiter().GetResult();
            commandBusMock.Verify(x => x.Push(It.IsAny<ProcessVideoCommand>()), Times.Once);
        }
    }
}