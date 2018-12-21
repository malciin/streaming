using Streaming.Domain.Models.DTO.Video;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Application.Query
{
    public interface IVideoQueries
    {
        Task<VideoBasicMetadataDTO> GetBasicVideoMetadataAsync(Guid VideoId);
    }
}
