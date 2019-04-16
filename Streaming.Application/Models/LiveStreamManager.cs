using Autofac;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Live;
using Streaming.Domain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Streaming.Application.Models
{
    public class LiveStreamManager : ILiveStreamManager
    {
        private readonly IComponentContext componentContext;
        private readonly ConcurrentDictionary<Guid, LiveStream> streams;

        public LiveStreamManager(IComponentContext componentContext)
        {
            streams = new ConcurrentDictionary<Guid, LiveStream>();
            this.componentContext = componentContext;
        }

        public void StartNewLiveStream(NewLiveStreamDTO newLiveStream)
        {
            streams.TryAdd(newLiveStream.LiveStreamId, new LiveStream
            {
                LiveStreamId = newLiveStream.LiveStreamId,
                Owner = newLiveStream.User,
                Started = DateTime.UtcNow,
                ManifestUrl = newLiveStream.ManifestUri
            });
        }

        public void FinishLiveStream(Guid streamId)
        {
            streams.TryRemove(streamId, out _);
        }

        public LiveStreamMetadataDTO Get(Guid liveStreamId)
            => new LiveStreamMetadataDTO
            {
                LiveStreamId = Guid.NewGuid(),
                ManifestUrl = "https://bitdash-a.akamaihd.net/content/MI201109210084_1/m3u8s/f08e80da-bf1d-4e3d-8899-f0f6155f6efa.m3u8",
                Started = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(30)),
                UserStarted = "malciin"
            };

        public IEnumerable<LiveStreamMetadataDTO> Get(EnumerableFilter<LiveStreamMetadataDTO> filter)
        {
            return filter(new List<LiveStreamMetadataDTO>
            {
                new LiveStreamMetadataDTO
                {
                    LiveStreamId = Guid.NewGuid(),
                    ManifestUrl = "https://bitdash-a.akamaihd.net/content/MI201109210084_1/m3u8s/f08e80da-bf1d-4e3d-8899-f0f6155f6efa.m3u8",
                    Started = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(30)),
                    UserStarted = "malciin"
                }
            });
        }
    }
}
