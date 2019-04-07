using Streaming.Domain.Models;
using System;
using System.Collections.Concurrent;

namespace Streaming.Application.Models
{
    public class LiveManager
    {
        private readonly ConcurrentDictionary<Guid, LiveInternalModel> streams;

        private class LiveInternalModel
        {
            public UserDetails User { get; set; }
            public string ManifestUrl { get; set; }
        }

        public LiveManager()
        {
            streams = new ConcurrentDictionary<Guid, LiveInternalModel>();
        }

        public void StartNewStream(Guid streamId, UserDetails user)
        {
            streams.TryAdd(streamId, new LiveInternalModel
            {
                User = user
            });
        }

        public void AddManifestUrl(Guid streamId, string manifestUrl)
        {
            streams[streamId].ManifestUrl = manifestUrl;
        }

        public void FinishStream(Guid streamId)
        {
            streams.TryRemove(streamId, out _);
        }
    }
}
