using Streaming.Application.Models.DTO.Live;
using System;
using System.Collections.Generic;
using System.IO;

namespace Streaming.Application.Query
{
    public interface ILiveQueries
    {
        IEnumerable<LiveDTO> Search(int offset, int howMuch);
        string GetManifest(Guid streamId);
        Stream GetVideoPart(Guid streamId, int part);
    }
}
