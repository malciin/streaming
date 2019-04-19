using Streaming.Application.Models;
using Streaming.Application.Models.DTO.Live;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Services
{
    public interface ILiveStreamManager
    {
        LiveStreamMetadataDTO Get(Guid liveStreamId);
        IEnumerable<LiveStreamMetadataDTO> Get(EnumerableFilter<LiveStreamMetadataDTO> filter);
        Task StartNewLiveStreamAsync(NewLiveStreamDTO newLiveStream);
        Task FinishLiveStreamAsync(Guid liveStreamId);
    }
}
