using MongoDB.Bson.Serialization;
using Streaming.Domain.Models;

namespace Streaming.Infrastructure.MongoDb.Mappings
{
    public static class VideoMappings
    {
        public static void Map()
        {
            BsonClassMap.RegisterClassMap<Video>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(x => x.VideoId);
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}
