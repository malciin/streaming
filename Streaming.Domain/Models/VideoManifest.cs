using Streaming.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Streaming.Domain.Models
{
    public class VideoManifest
    {
        public class PartContext
        {
            public Guid VideoId;
            public TimeSpan PartLength;
            public int PartNumber;
        }

        private Guid mediaId;
        private List<TimeSpan> partsLength;

        private VideoManifest()
        {
            partsLength = new List<TimeSpan>();
        }

        public static VideoManifest FromByteArray(byte[] array)
        {
            var manifest = new VideoManifest();
            using (var binaryReader = new BinaryReader(new MemoryStream(array)))
            {
                var guidByteArray = new byte[16];
                binaryReader.Read(guidByteArray, 0, 16);
                manifest.mediaId = new Guid(guidByteArray);
                while(binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                {
                    manifest.partsLength.Add(TimeSpan.FromTicks(binaryReader.ReadInt64()));
                }
            }
            return manifest;
        }

        public static VideoManifest Create(Guid mediaId)
        {
            var manifest = new VideoManifest();
            manifest.mediaId = mediaId;
            return manifest;
        }

        public void AddPart(TimeSpan partLength)
            => partsLength.Add(partLength);

        public byte[] ToByteArray()
        {
            var size = mediaId.ToByteArray().Length + sizeof(long) * partsLength.Count;
            using (var memoryStream = new MemoryStream(size))
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(mediaId.ToByteArray());
                foreach (var length in partsLength)
                    binaryWriter.Write(length.Ticks);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Generate manifest string with URL from function
        /// </summary>
        /// <param name="partUrlStrategy">Function that get urls for specific video parts</param>
        /// <param name="finishedManifest">Indicates if the manifest file is finished - should '#EXT-X-ENDLIST' be added to manifest file</param>
        /// <returns></returns>
        public string GenerateManifest(Func<PartContext, string> partUrlStrategy, bool finishedManifest = true)
        {
            var manifest = new StringBuilder();

            int counter = 0;
            TimeSpan maxPartLength = TimeSpan.FromSeconds(0);
            foreach(var partLength in partsLength)
            {
                if (maxPartLength < partLength)
                    maxPartLength = partLength;

                manifest.AppendLine($"#EXTINF:{partLength.TotalSeconds}");
                manifest.AppendLine(partUrlStrategy(new PartContext
                {
                    VideoId = mediaId,
                    PartLength = partLength,
                    PartNumber = counter++,
                }));
            }
            if (finishedManifest)
                manifest.AppendLine("#EXT-X-ENDLIST");

            manifest.PrependLine("#EXT-X-MEDIA-SEQUENCE:0");
            manifest.PrependLine($"#EXT-X-TARGETDURATION:{maxPartLength.TotalSeconds}");
            manifest.PrependLine("#EXT-X-VERSION:3");
            manifest.PrependLine("#EXTM3U");

            return manifest.ToString();
        }
    }
}
