using System;
using System.Collections.Generic;
using Streaming.Domain.Models;

namespace Streaming.Infrastructure.MongoDb
{
    public static class MongoDbNames
    {
        public const string DatabaseName = "streaming";
        public static readonly Dictionary<Type, string> CollectionNames = new Dictionary<Type, string>
        {
            [typeof(Video)] = "Videos",
            [typeof(LiveStream)] = "LiveStreams"
        };
    }
}
