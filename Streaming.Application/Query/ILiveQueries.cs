using Streaming.Application.Models.DTO.Live;
using System.Collections.Generic;

namespace Streaming.Application.Query
{
    public interface ILiveQueries
    {
        IEnumerable<LiveStreamMetadataDTO> Search(int offset, int howMuch);
    }
}
