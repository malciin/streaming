using Streaming.Application.DTO.Video;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Query
{
    public interface IVideoQueries
    {
        Task<VideoBasicMetadataDTO> GetBasicVideoMetadataAsync(Guid VideoId);
		Task<IEnumerable<VideoBasicMetadataDTO>> SearchAsync(VideoSearchDTO Search);
		Task<string> GetVideoManifestAsync(Guid VideoId);
		Task<Stream> GetVideoPartAsync(Guid VideoId, int Part);
	}
}