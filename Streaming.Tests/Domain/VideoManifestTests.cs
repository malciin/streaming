using System;
using System.Collections.Generic;
using NUnit.Framework;
using Streaming.Domain.Models;

namespace Streaming.Tests.Domain
{
    public class VideoManifestTests
    {
        private static IEnumerable<VideoManifest> IsByteSerializationWorksData
        {
            get
            {
                yield return VideoManifest.Create(Guid.NewGuid());
                yield return VideoManifest.Create(Guid.NewGuid())
                    .AddPart(TimeSpan.FromSeconds(6))
                    .AddPart(TimeSpan.FromSeconds(5))
                    .AddPart(TimeSpan.FromSeconds(4))
                    .AddPart(TimeSpan.FromSeconds(3))
                    .AddPart(TimeSpan.FromSeconds(2));
            }
        }

        private static string UrlStrategy(VideoManifest.PartContext context) 
            => $"{context.VideoId}__{context.PartLength}__{context.PartNumber}";

        [TestCaseSource(nameof(IsByteSerializationWorksData))]
        public void Is_Byte_Serialization_Works(VideoManifest manifest)
        {
            var bytes = manifest.ToByteArray();
            var deserializedManifest = VideoManifest.FromByteArray(bytes);

            var originalManifest = manifest.GenerateManifest(UrlStrategy);
            var manifestFromDeserializedObject = deserializedManifest.GenerateManifest(UrlStrategy);
            
            Assert.AreEqual(originalManifest, manifestFromDeserializedObject);
        }

        private static IEnumerable<(VideoManifest manifest, string expectedResult)> IsGenerateManifestWorksData
        {
            get
            {
                yield return (VideoManifest.Create(Guid.Parse("F0D76A21-8F3C-42D9-939E-72EB0B4E7B46")),
                    String.Format("#EXTM3U{0}" +
                                  "#EXT-X-VERSION:3{0}" +
                                  "#EXT-X-TARGETDURATION:0{0}" +
                                  "#EXT-X-MEDIA-SEQUENCE:0{0}" +
                                  "#EXT-X-ENDLIST{0}", Environment.NewLine));
                yield return (VideoManifest.Create(Guid.Parse("C5D0A04C-F91C-4764-94C7-2BF7D8050373"))
                    .AddPart(TimeSpan.FromSeconds(6))
                    .AddPart(TimeSpan.FromSeconds(5))
                    .AddPart(TimeSpan.FromSeconds(4))
                    .AddPart(TimeSpan.FromSeconds(3))
                    .AddPart(TimeSpan.FromSeconds(2)), 
                    String.Format("#EXTM3U{0}#EXT-X-VERSION:3{0}#EXT-X-TARGETDURATION:6{0}" +
                                  "#EXT-X-MEDIA-SEQUENCE:0{0}" +
                                  "#EXTINF:6{0}c5d0a04c-f91c-4764-94c7-2bf7d8050373__00:00:06__0{0}" +
                                  "#EXTINF:5{0}c5d0a04c-f91c-4764-94c7-2bf7d8050373__00:00:05__1{0}" +
                                  "#EXTINF:4{0}c5d0a04c-f91c-4764-94c7-2bf7d8050373__00:00:04__2{0}" +
                                  "#EXTINF:3{0}c5d0a04c-f91c-4764-94c7-2bf7d8050373__00:00:03__3{0}" +
                                  "#EXTINF:2{0}c5d0a04c-f91c-4764-94c7-2bf7d8050373__00:00:02__4{0}" +
                                  "#EXT-X-ENDLIST{0}", Environment.NewLine));
            }
        }

        [TestCaseSource(nameof(IsGenerateManifestWorksData), Category = "Is generate manifest works")]
        public void Is_GenerateManifest_Works((VideoManifest manifest, string expectedResult) input)
        {
            var generatedManifest = input.manifest.GenerateManifest(UrlStrategy);
            Assert.AreEqual(generatedManifest, input.expectedResult);
        }
    }
}