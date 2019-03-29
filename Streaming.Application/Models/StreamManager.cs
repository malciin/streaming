using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Streaming.Application.Models
{
    public class StreamManager
    {
        private readonly ConcurrentDictionary<Guid, string> streamKeyMappings;
        private readonly ConcurrentDictionary<string, StreamInternalModel> memoryModel;

        private class StreamInternalModel
        {
            private readonly int maxTsFiles = 5;
            // Check EXT-X-MEDIA-SEQUENCE for details
            public int MediaSequence { get; private set; }
            private ConcurrentDictionary<int, (TimeSpan, bool, byte[])> streamParts;
            private ConcurrentQueue<int> partsQueue;

            public StreamInternalModel()
            {
                MediaSequence = 0;
                streamParts = new ConcurrentDictionary<int, (TimeSpan, bool, byte[])>();
                partsQueue = new ConcurrentQueue<int>();
            }
            
            public Stream Get(int part)
                => new MemoryStream(streamParts[part].Item3, writable: false);

            public bool IsDiscontinuted(int part)
                => streamParts[part].Item2;

            public void Push((TimeSpan, bool, byte[]) part)
            {
                streamParts.TryAdd(MediaSequence, part);
                partsQueue.Enqueue(MediaSequence);
                MediaSequence++;
                if (partsQueue.Count > maxTsFiles)
                {
                    PopLast();
                }
            }

            void PopLast()
            {
                partsQueue.TryDequeue(out int partId);
                streamParts.TryRemove(partId, out _);
            }

            public TimeSpan Length(int part)
                => streamParts[part].Item1;
        }

        public StreamManager()
        {
            streamKeyMappings = new ConcurrentDictionary<Guid, string>();
            memoryModel = new ConcurrentDictionary<string, StreamInternalModel>();
        }

        private string getStreamKey(Guid streamId)
            => streamKeyMappings[streamId];

        public void StartNewStream(string streamKey)
        {
            if (streamKeyMappings.ContainsKey(Guid.Parse("00000000-0001-0000-0000-000000000000")))
            {
                memoryModel.Clear();
                streamKeyMappings.Clear();
            }
            streamKeyMappings.TryAdd(Guid.Parse("00000000-0001-0000-0000-000000000000"), streamKey);
            memoryModel.TryAdd(streamKey, new StreamInternalModel());
        }

        public void Upload(string streamKey, Stream part, string fileManifestDetails)
        {
            var buffer = new byte[part.Length];
            part.Read(buffer, 0, buffer.Length);
            var matched = Regex.Match(fileManifestDetails, @"EXTINF:(\d+\.\d+)");
            memoryModel[streamKey].Push((TimeSpan.FromSeconds(double.Parse(matched.Groups[1].Value)), fileManifestDetails.Contains("#EXT-X-DISCONTINUITY"), buffer));
        }

        public Stream GetPart(Guid streamId, int part)
            => memoryModel[getStreamKey(streamId)].Get(part);

        public void FinishStream(string streamKey)
        {
            var toDelKey = streamKeyMappings.Where(x => x.Value == streamKey).First().Key;
            streamKeyMappings.TryRemove(toDelKey, out _);
        }

        public string GenerateManifest(Guid streamId, Func<Guid, int, string> urlStrategy)
        {
            var streamKey = streamKeyMappings[streamId];
            var streamMemoryModel = memoryModel[streamKey];
            var manifestBuilder = new StringBuilder();
            manifestBuilder.AppendLine("#EXTM3U");
            manifestBuilder.AppendLine("#EXT-X-VERSION:3");
            manifestBuilder.AppendLine($"#EXT-X-TARGETDURATION:3");
            manifestBuilder.AppendLine($"#EXT-X-MEDIA-SEQUENCE:{Math.Max(0, streamMemoryModel.MediaSequence- maxTsFiles)}");

            for (int i = Math.Max(0, streamMemoryModel.MediaSequence - maxTsFiles); i<streamMemoryModel.MediaSequence; i++)
            {
                if (streamMemoryModel.IsDiscontinuted(i))
                    manifestBuilder.AppendLine("#EXT-X-DISCONTINUITY");

                manifestBuilder.AppendLine($"#EXTINF:{streamMemoryModel.Length(i).TotalSeconds}");
                manifestBuilder.AppendLine(urlStrategy(streamId, i));
            }
            return manifestBuilder.ToString();
        }

        public IEnumerable<Guid> GetRunningVideos()
            => streamKeyMappings.Keys.AsEnumerable();
    }
}
