using Streaming.Application.Models.Repository.Video;
using Streaming.Domain.Models;
using System;
using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Repositories
{
	public interface IVideoRepository : IFilterableRepository<Video>
	{
		Task AddAsync(Video video);
        Task UpdateAsync(UpdateVideoInfo updateVideoInfo);
        Task UpdateAsync(UpdateVideoAfterProcessing updateVideoAfterProcessing);
        Task DeleteAsync(Guid videoId);
	}
}
