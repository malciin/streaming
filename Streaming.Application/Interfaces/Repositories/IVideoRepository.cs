using Streaming.Application.Models.DTO.Video;
using Streaming.Application.Models.Repository.Video;
using Streaming.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Repositories
{
	public interface IVideoRepository
	{
		Task<Video> GetAsync(Guid videoId);
		Task<IEnumerable<Video>> SearchAsync(VideoSearchDTO search);
		Task AddAsync(Video video);
        Task UpdateAsync(UpdateVideoInfo updateVideoInfo);
        Task UpdateAsync(UpdateVideoAfterProcessing updateVideoAfterProcessing);
        Task DeleteAsync(Guid VideoId);
        Task CommitAsync();
	}
}
