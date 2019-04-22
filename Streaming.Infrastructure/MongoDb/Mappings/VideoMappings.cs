using System;
using MongoDB.Bson.Serialization;
using Streaming.Domain.Models;

namespace Streaming.Infrastructure.MongoDb.Mappings
{
    public static class VideoMappings
    {
        private class VideoManifestSerializer : IBsonSerializer<VideoManifest>
        {
            public Type ValueType => typeof(VideoManifest);

            public VideoManifest Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
                => Deserialize(context, args) as VideoManifest;

            public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
                => Serialize(context, args, value as VideoManifest);

            public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, VideoManifest value)
            {
                if (value != null)
                    context.Writer.WriteBytes(value.ToByteArray());
                else
                    context.Writer.WriteNull();
            }

            object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            {
                if (context.Reader.CurrentBsonType == MongoDB.Bson.BsonType.Null)
                {
                    context.Reader.ReadNull();
                    return null;
                }
                return VideoManifest.FromByteArray(context.Reader.ReadBinaryData().AsByteArray);
            }
        }

        public static void Map()
        {
            BsonClassMap.RegisterClassMap<Video>(cm =>
            {
                cm.AutoMap();
                cm.GetMemberMap(x => x.VideoManifest)
                    .SetSerializer(new VideoManifestSerializer());
                cm.MapIdMember(x => x.VideoId);
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}
