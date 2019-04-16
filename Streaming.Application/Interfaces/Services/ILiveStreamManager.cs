using Streaming.Application.Models;
using Streaming.Application.Models.DTO.Live;
using System;
using System.Collections.Generic;

namespace Streaming.Application.Interfaces.Services
{
    public interface ILiveStreamManager
    {
        LiveStreamMetadataDTO Get(Guid liveStreamId);
        IEnumerable<LiveStreamMetadataDTO> Get(EnumerableFilter<LiveStreamMetadataDTO> filter);
        void StartNewLiveStream(NewLiveStreamDTO newLiveStream);
        void FinishLiveStream(Guid liveStreamId);
    }
}
