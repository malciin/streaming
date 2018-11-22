using MongoDB.Bson.Serialization;
using Streaming.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Mappings
{
    public static class MongoDbMappings
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
