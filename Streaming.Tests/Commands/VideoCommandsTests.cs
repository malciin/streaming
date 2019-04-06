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
using System.Reflection;

namespace Streaming.Tests.Commands
{
    class VideoCommandsTests
    {
        private List<Video> videos;
        private ContainerBuilder containerBuilder;

		private Mock<IVideoRepository> videoRepositoryMock = new Mock<IVideoRepository>();
		private Mock<IProcessVideoService> processVideoService = new Mock<IProcessVideoService>();
		private Mock<IVideoFileInfoService> videoFileInfoService = new Mock<IVideoFileInfoService>();
		private Mock<IMessageSignerService> messageSignerService = new Mock<IMessageSignerService>();

		public IComponentContext BuildContext()
		{
			var mockObjects = this.GetType()
									.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
									.Select(x => x.GetValue(this))
									.Where(x => x.GetType().IsAssignableTo<Mock>());

			var mockedObjects = mockObjects.Select(x => x.GetType()
														 .GetProperty("Object", x.GetType().GenericTypeArguments[0])
														 .GetValue(x));

			foreach (var mockedObject in mockedObjects)
			{
				var mockedObjectType = mockedObject.GetType();
				containerBuilder.Register(x => mockedObject).AsImplementedInterfaces();
			}
			return containerBuilder.Build();
		}

		[SetUp]
        public void SetUp()
        {
            videos = new List<Video>();
			containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterType<CommandDispatcher>().AsImplementedInterfaces().SingleInstance();
			containerBuilder.RegisterType<CommandBus>().AsImplementedInterfaces().SingleInstance();
            containerBuilder.RegisterModule<StrategiesModule>();

			containerBuilder.RegisterUnused(typeof(IDirectoriesSettings));
            processVideoService = new Mock<IProcessVideoService>();

			/// MOCKS ///
			videoFileInfoService.Setup(x => x.GetDetailsAsync(It.IsAny<string>())).Returns(Task.FromResult(new VideoFileDetailsDTO
			{
				Video = new VideoFileDetailsDTO.VideoDetailsDTO
				{
					Codec = VideoCodec.h264
				}
			}));

			processVideoService.Setup(x => x.SupportedVideoCodecs()).Returns(new List<VideoCodec>
            {
                VideoCodec.h264
            });

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

			messageSignerService.Setup(x => x.GetMessage(It.IsAny<byte[]>())).Returns(Guid.NewGuid().ToByteArray());
        }

        [Test]
        public void UploadVideoCommand_Unsupported_Video_Codec_Should_Throw_NotSupportedException()
        {
            videoFileInfoService.Setup(x => x.GetDetailsAsync(It.IsAny<string>())).Returns(Task.FromResult(new VideoFileDetailsDTO
            {
                Video = new VideoFileDetailsDTO.VideoDetailsDTO
                {
                    Codec = VideoCodec.mpeg1video
                }
            }));
            var ctx = BuildContext();

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
			containerBuilder.RegisterType<UploadVideoHandler>().AsImplementedInterfaces();
            var ctx = BuildContext();

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
