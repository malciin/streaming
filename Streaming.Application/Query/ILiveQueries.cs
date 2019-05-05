using Streaming.Application.Models.DTO.Live;
using System;
using System.Collections.Generic;

namespace Streaming.Application.Query
{
    public interface ILiveQueries
    {
        LiveStreamMetadataDTO Get(Guid liveStreamId);
        IEnumerable<LiveStreamMetadataDTO> Get(int offset, int howMuch);
    }
}
