using Streaming.Application.Models.DTO.Video;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Streaming.Application.Interfaces.Models;
using Streaming.Domain.Models;

namespace Streaming.Application.Query
{
    public interface IVideoQueries
    {
        Task<VideoMetadataDTO> GetBasicVideoMetadataAsync(Guid videoId);
		Task<IPackage<VideoMetadataDTO>> SearchAsync(VideoSearchDTO search, Expression<Func<Video, object>> orderByDescending);
		Task<string> GetVideoManifestAsync(Guid videoId);
		Task<Stream> GetVideoPartAsync(Guid videoId, int part);
		
		
	}
}