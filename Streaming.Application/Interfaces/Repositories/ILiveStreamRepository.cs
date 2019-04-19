using Streaming.Domain.Models;
using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Repositories
{
    public interface ILiveStreamRepository : IFilterableRepository<LiveStream>
    {
        Task AddAsync(LiveStream liveStream);
        Task CommitAsync();
    }
}
