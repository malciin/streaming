using Streaming.Application.Models.DTO.Live;
using System.Collections.Generic;

namespace Streaming.Application.Query
{
    public interface ILiveQueries
    {
        IEnumerable<LiveMetadataDTO> Search(int offset, int howMuch);
    }
}
