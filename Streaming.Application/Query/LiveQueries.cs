using System;
using System.Collections.Generic;
using System.Linq;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Live;

namespace Streaming.Application.Query
{
    public class LiveQueries : ILiveQueries
    {
        private readonly ILiveStreamManager streamManager;

        public LiveQueries(ILiveStreamManager streamManager)
        {
            this.streamManager = streamManager;
        }

        public LiveStreamMetadataDTO Get(Guid liveStreamId)
            => streamManager.Get(liveStreamId);

        public IEnumerable<LiveStreamMetadataDTO> Get(int offset, int howMuch)
            => streamManager.Get(x => x.OrderByDescending(y => y.Started).Skip(offset).Take(howMuch));
    }
}
