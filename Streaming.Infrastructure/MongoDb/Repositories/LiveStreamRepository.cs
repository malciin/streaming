using MongoDB.Driver;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Domain.Models;

namespace Streaming.Infrastructure.MongoDb.Repositories
{
    public class LiveStreamRepository : _GenericRepository<LiveStream>, ILiveStreamRepository
    {
        public LiveStreamRepository(IMongoCollection<LiveStream> collection) : base(collection)
        {
        }
    }
}
