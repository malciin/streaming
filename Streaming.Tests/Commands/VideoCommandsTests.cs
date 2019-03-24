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
using Streaming.Application.Models.Repository.Video;
using Streaming.Domain.Enums;
using Streaming.Domain.Models;
using Streaming.Infrastructure.IoC;
using Streaming.Tests.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Tests.Commands
{
    class VideoCommandsTests
    {
        private List<Video> videos;
        private ContainerBuilder containerBuilder;

        private Mock<IVideoRepository> videoRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            videos = new List<Video>();
            containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<CommandModule>();
            containerBuilder.RegisterModule<StrategiesModule>();

            containerBuilder.RegisterUnused(typeof(IDirectoriesSettings));

            var processVideoService = new Mock<IProcessVideoService>();
            processVideoService.Setup(x => x.SupportedVideoCodecs()).Returns(new List<VideoCodec>
            {
                VideoCodec.h264
            });
            containerBuilder.Register(x => processVideoService.Object).AsImplementedInterfaces();

            videoRepositoryMock = new Mock<IVideoRepository>();
            videoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Video>())).Returns((Video vid) =>
            {
                videos.Add(vid);
                return Task.FromResult(0);
            });

            videoRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>())).Returns((Guid id) =>
            {
                videos = videos.Where(x => x.VideoId != id).ToList();
                return Task.FromResult(0);
            });

            videoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<UpdateVideoInfo>())).Returns((UpdateVideoInfo vid) =>
            {
                var video = videos.Where(x => x.VideoId == vid.UpdateByVideoId && vid.UpdateByUserIdentifier == x.Owner.UserId).FirstOrDefault();
                if (video != null)
                {
                    video.Title = vid.NewVideoTitle;
                    video.Description = vid.NewVideoDescription;
                }
                return Task.FromResult(0);
            });
            containerBuilder.Register(x => videoRepositoryMock.Object).AsImplementedInterfaces();

            var messageSignerService = new Mock<IMessageSignerService>();
            messageSignerService.Setup(x => x.GetMessage(It.IsAny<byte[]>())).Returns(Guid.NewGuid().ToByteArray());
            containerBuilder.Register(x => messageSignerService.Object).AsImplementedInterfaces();
        }

        private void addDefaultVideoFileInfoService()
        {
            var videoFileInfoService = new Mock<IVideoFileInfoService>();
            videoFileInfoService.Setup(x => x.GetDetailsAsync(It.IsAny<string>())).Returns(Task.FromResult(new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Codec = VideoCodec.h264
                }
            }));
            containerBuilder.Register(x => videoFileInfoService.Object).AsImplementedInterfaces();
        }

        [Test]
        public void UploadVideoCommand_Unsupported_Video_Codec_Should_Throw_NotSupportedException()
        {
            var videoFileInfoService = new Mock<IVideoFileInfoService>();
            videoFileInfoService.Setup(x => x.GetDetailsAsync(It.IsAny<string>())).Returns(Task.FromResult(new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Codec = VideoCodec.mpeg1video
                }
            }));
            containerBuilder.Register(x => videoFileInfoService.Object).AsImplementedInterfaces();

            var ctx = containerBuilder.Build();

            var repo = ctx.Resolve<IVideoRepository>();

            Assert.Throws<NotSupportedException>(() => ctx.Resolve<ICommandDispatcher>().HandleAsync(new UploadVideoCommand
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
            }).GetAwaiter().GetResult());
        }

        [Test]
        public void UploadVideoCommand_Adding_Video_Works()
        {
            addDefaultVideoFileInfoService();
            var ctx = containerBuilder.Build();

            ctx.Resolve<ICommandDispatcher>().HandleAsync(new UploadVideoCommand
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

            videoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Video>()), Times.Once);
            Assert.AreEqual(1, videos.Count);
            Assert.AreEqual("Title", videos[0].Title);
            Assert.AreEqual("Desc", videos[0].Description);
            Assert.AreEqual("email@gmail.com", videos[0].Owner.Email);
            Assert.AreEqual("id", videos[0].Owner.UserId);
            Assert.AreEqual("nick", videos[0].Owner.Nickname);
            Assert.AreEqual(VideoState.Fresh, videos[0].State);
        }
    }
}
