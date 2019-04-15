using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Live;

namespace Streaming.Application.Query
{
    public class LiveQueries : ILiveQueries
    {
        private readonly ILiveStreamManager streamManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        public LiveQueries(ILiveStreamManager streamManager, IHttpContextAccessor httpContextAccessor)
        {
            this.streamManager = streamManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<LiveStreamMetadataDTO> Search(int offset, int howMuch)
        {
            throw new NotImplementedException();
        }
    }
}
