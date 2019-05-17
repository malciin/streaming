using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Moq;
using NUnit.Framework;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Live;
using Streaming.Common.Extensions;
using Streaming.Domain.Models;
using Streaming.Infrastructure.IoC;
using Streaming.Tests.Mocks;

namespace Streaming.Tests.Services
{
    public class LiveStreamManagerTests
    {
        private List<LiveStream> liveStreamsInDatabase;
        private Mock<IPastLiveStreamRepository> liveStreamRepositoryMock;

        private ILiveStreamManager LiveStreamManager { get; set; }

        [SetUp] 
        public void Setup()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ServicesModule>();
            containerBuilder.RegisterModule<MappingModule>();
            
            liveStreamsInDatabase = new List<LiveStream>();
            liveStreamRepositoryMock = LiveStreamRepositoryMock.CreateForData(liveStreamsInDatabase);
            containerBuilder.Register(x => liveStreamRepositoryMock.Object).AsImplementedInterfaces();
            
            var context = containerBuilder.Build();
            LiveStreamManager = context.Resolve<ILiveStreamManager>();
        }

        [Test]
        public async Task Correct_LiveStream_Start_Datetime_When_Adding_LiveStream()
        {
            var newStreamGuid = Guid.NewGuid();
            var beforeCommand = DateTime.UtcNow;
            await AddDefaultLiveStream(newStreamGuid);
            var afterCommand = DateTime.UtcNow;

            var addedStream = LiveStreamManager.Get(x => x.Where(y => y.LiveStreamId == newStreamGuid)).First();
            Assert.IsTrue(addedStream.Started.IsWithin(beforeCommand, afterCommand));
        }

        [Test]
        public async Task Finish_LiveStream_Should_Add_That_Stream_To_LiveStreams_Repository()
        {
            var newStreamGuid = Guid.NewGuid();
            await AddDefaultLiveStream(newStreamGuid);
            await LiveStreamManager.FinishLiveStreamAsync(newStreamGuid);

            liveStreamRepositoryMock.Verify(x => x.AddAsync(It.IsAny<LiveStream>()), Times.Once());

            Assert.NotNull(liveStreamRepositoryMock.Object
                .GetSingleAsync(x => x.LiveStreamId == newStreamGuid)
                .GetAwaiter().GetResult());
        }

        [Test]
        public async Task Finish_LiveStream_Should_Set_Proper_End_Datetime()
        {
            var newStreamGuid = Guid.NewGuid();
            await AddDefaultLiveStream(newStreamGuid);

            var beforeFinishing = DateTime.UtcNow;
            await LiveStreamManager.FinishLiveStreamAsync(newStreamGuid);
            var afterFinishing = DateTime.UtcNow;

            var finishedLiveStream = liveStreamRepositoryMock.Object.GetSingleAsync(x => x.LiveStreamId == newStreamGuid).GetAwaiter().GetResult();
            Assert.IsTrue(finishedLiveStream.Ended.IsWithin(beforeFinishing, afterFinishing));
        }

        [Test]
        public async Task Finish_LiveStream_Should_Delete_It_From_Current_Streams()
        {
            var newStreamGuid = Guid.NewGuid();
            await AddDefaultLiveStream(newStreamGuid);
            await LiveStreamManager.FinishLiveStreamAsync(newStreamGuid);
            
            Assert.AreEqual(0, LiveStreamManager.Get(x => x).Count());
        }

        private async Task AddDefaultLiveStream(Guid liveStreamGuid)
        {
            await LiveStreamManager.StartNewLiveStreamAsync(new NewLiveStreamDTO
            {
                User = new UserDetails
                {
                    Nickname = "nickname",
                    Email = "email@email.com",
                    UserId = Guid.NewGuid().ToString()
                },
                ManifestUri = new Uri("http://someuri.com"),
                LiveStreamId = liveStreamGuid
            });
        }
    }
}