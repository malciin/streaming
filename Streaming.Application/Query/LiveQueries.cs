using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Streaming.Application.Models;
using Streaming.Application.Models.DTO.Live;

namespace Streaming.Application.Query
{
    public class LiveQueries : ILiveQueries
    {
        private readonly LiveManager streamManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        public LiveQueries(LiveManager streamManager, IHttpContextAccessor httpContextAccessor)
        {
            this.streamManager = streamManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<LiveMetadataDTO> Search(int offset, int howMuch)
        {
            throw new NotImplementedException();
        }
    }
}
