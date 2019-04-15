using Autofac;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Live;
using Streaming.Domain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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

        public void StartNewStream(NewLiveStreamDTO newLiveStream)
        {
            streams.TryAdd(newLiveStream.LiveStreamId, new LiveStream
            {
                LiveStreamId = newLiveStream.LiveStreamId,
                Owner = newLiveStream.User,
                Started = DateTime.UtcNow,
                ManifestUrl = newLiveStream.ManifestUri
            });
        }

        public void FinishStream(Guid streamId)
        {
            streams.TryRemove(streamId, out _);
        }

        public IEnumerable<LiveStreamMetadataDTO> GetAll()
            => streams.Values.Select(x => new LiveStreamMetadataDTO
            {
                LiveStreamId = x.LiveStreamId,
                ManifestUrl = x.ManifestUrl.ToString()
            });
    }
}
