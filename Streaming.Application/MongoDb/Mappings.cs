using MongoDB.Bson.Serialization;
using Streaming.Domain.Models;

namespace Streaming.Application.MongoDb
{
    public static class Mappings
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
