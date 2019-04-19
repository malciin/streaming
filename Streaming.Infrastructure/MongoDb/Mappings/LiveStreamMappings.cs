using MongoDB.Bson.Serialization;
using Streaming.Domain.Models;

namespace Streaming.Infrastructure.MongoDb.Mappings
{
    public static class LiveStreamMappings
    {
        public static void Map()
        {
            BsonClassMap.RegisterClassMap<LiveStream>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(x => x.LiveStreamId);
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}
