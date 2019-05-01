using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        ILiveStreamRepository LiveStreams { get; }
        IVideoRepository Videos { get; }
        Task CommitAsync();
    }
}
