using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        IPastLiveStreamRepository PastLiveStreams { get; }
        IVideoRepository Videos { get; }
        Task CommitAsync();
    }
}
