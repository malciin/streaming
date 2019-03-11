using Streaming.Application.DTO.Video;
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
		Task UpdateAsync(Video video);
		Task CommitAsync();
	}
}
