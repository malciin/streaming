using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Streaming.Application.Models;
using Streaming.Application.Models.DTO.Live;
using Streaming.Common.Helpers;

namespace Streaming.Application.Query
{
    public class LiveQueries : ILiveQueries
    {
        private readonly StreamManager streamManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        public LiveQueries(StreamManager streamManager, IHttpContextAccessor httpContextAccessor)
        {
            this.streamManager = streamManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GetManifest(Guid streamId)
            => streamManager.GenerateManifest(streamId, (id, part) =>
                 UrlHelper.GetHostUrl(httpContextAccessor.HttpContext) + $"/Live/{id}/{part}"
            );

        public Stream GetVideoPart(Guid streamId, int part)
            => streamManager.GetPart(streamId, part);

        public IEnumerable<LiveDTO> Search(int offset, int howMuch)
        {
            return streamManager.GetRunningVideos().Select(x => new LiveDTO
            {
                StreamId = x
            });
        }
    }
}
