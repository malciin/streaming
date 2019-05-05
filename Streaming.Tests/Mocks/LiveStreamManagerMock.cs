using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Streaming.Application;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models;
using Streaming.Application.Models.DTO.Live;
using Streaming.Domain.Models;

namespace Streaming.Tests.Mocks
{
    public static class LiveStreamManagerMock
    {
        private static readonly Mapper Mapper = new Mapper();
        public static Mock<ILiveStreamManager> CreateForData(ICollection<LiveStream> liveStreams)
        {
            var mock = new Mock<ILiveStreamManager>();

            mock.Setup(x => x.Get(It.IsAny<EnumerableFilter<LiveStream>>())).Returns(
                (EnumerableFilter<LiveStream> filter) =>
                    filter(liveStreams).Select(x => Mapper.MapLiveStreamMetadataDTO(x)));

            mock.Setup(x => x.GetSingle(It.IsAny<Guid>()))
                .Returns((Guid id) => Mapper.MapLiveStreamMetadataDTO(liveStreams.First(x => x.LiveStreamId == id)));

            mock.Setup(x => x.StartNewLiveStreamAsync(It.IsAny<NewLiveStreamDTO>()))
                .Returns((NewLiveStreamDTO newLiveStream) =>
                {
                    liveStreams.Add(new LiveStream
                    {
                        Title = $"Title for stream number {liveStreams.Count + 1}",
                        LiveStreamId = newLiveStream.LiveStreamId,
                        Started = DateTime.Now,
                        Owner = newLiveStream.User,
                        ManifestUrl = newLiveStream.ManifestUri
                    });
                    return Task.FromResult(0);
                });

            return mock;
        }
    }
}