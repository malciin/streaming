using MongoDB.Driver;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Domain.Models;

namespace Streaming.Infrastructure.MongoDb.Repositories
{
    public class PastLiveStreamRepository : GenericRepository<LiveStream>, IPastLiveStreamRepository, IRepositoryMarker
    {
        public PastLiveStreamRepository(IMongoCollection<LiveStream> collection) : base(collection)
        {
        }
    }
}
