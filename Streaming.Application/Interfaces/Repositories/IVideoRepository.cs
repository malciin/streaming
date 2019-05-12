using Streaming.Domain.Models;
using System;
using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Repositories
{
	public interface IVideoRepository : IFilterableRepository<Video>
	{
		Task AddAsync(Video video);
        Task UpdateAsync(Video video);
        Task DeleteAsync(Guid videoId);
	}
}
