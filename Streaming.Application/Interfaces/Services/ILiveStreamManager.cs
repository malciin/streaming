using Streaming.Application.Models.DTO.Live;
using System;
using System.Collections.Generic;

namespace Streaming.Application.Interfaces.Services
{
    public interface ILiveStreamManager
    {
        IEnumerable<LiveStreamMetadataDTO> GetAll();
        void StartNewStream(NewLiveStreamDTO newLiveStream);
        void FinishStream(Guid liveStreamId);
    }
}
