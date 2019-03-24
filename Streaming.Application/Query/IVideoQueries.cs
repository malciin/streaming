using Streaming.Application.Models.DTO.Video;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Query
{
    public interface IVideoQueries
    {
        Task<VideoMetadataDTO> GetBasicVideoMetadataAsync(Guid VideoId);
		Task<IEnumerable<VideoMetadataDTO>> SearchAsync(VideoSearchDTO Search);
		Task<string> GetVideoManifestAsync(Guid VideoId);
		Task<Stream> GetVideoPartAsync(Guid VideoId, int Part);
	}
}