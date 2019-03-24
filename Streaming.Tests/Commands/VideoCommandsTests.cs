using Autofac;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Streaming.Application.Commands;
using Streaming.Application.Commands.Video;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using Streaming.Application.Models.DTO.Video;
using Streaming.Application.Models.Enum;
using Streaming.Infrastructure.IoC;
using Streaming.Tests.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Streaming.Tests.Commands
{
    class VideoCommandsTests
    {
        private ContainerBuilder containerBuilder;
        [SetUp]
        public void SetUp()
        {
            containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<CommandModule>();
            containerBuilder.RegisterModule<StrategiesModule>();

            var processVideoService = new Mock<IProcessVideoService>();
            processVideoService.Setup(x => x.SupportedVideoCodecs()).Returns(new List<VideoCodec>
            {
                VideoCodec.h264
            });
            containerBuilder.Register(x => processVideoService.Object).AsImplementedInterfaces();

            var messageSignerService = new Mock<IMessageSignerService>();
            messageSignerService.Setup(x => x.GetMessage(It.IsAny<byte[]>())).Returns(Guid.NewGuid().ToByteArray());
            containerBuilder.Register(x => messageSignerService.Object).AsImplementedInterfaces();
        }

        [Test]
        public void Unsupported_Video_Codec_Should_Throw_NotSupportedException()
        {
            containerBuilder.RegisterUnusedServices(typeof(IVideoRepository), typeof(IDirectoriesSettings));

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
    }
}
