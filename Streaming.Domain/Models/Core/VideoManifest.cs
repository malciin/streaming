using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Streaming.Domain.Models.Core
{
    public class VideoManifest
    {
        private StringBuilder manifest;
        private int parts = 0;
        private bool headersSet;
        private bool finished;

        public int PartsCount => parts;

        public VideoManifest()
        {
            headersSet = false;
            finished = false;
            manifest = new StringBuilder();
        }

        public VideoManifest SetHeaders(int TargetDurationSeconds)
        {
            if (headersSet)
                throw new FormatException($"Manifest can't have more than one header");

            manifest.AppendLine("#EXTM3U");
            manifest.AppendLine("#EXT-X-VERSION:3");
            manifest.AppendLine($"#EXT-X-TARGETDURATION:{TargetDurationSeconds}");
            manifest.AppendLine("#EXT-X-MEDIA-SEQUENCE:0");
            headersSet = true;
            return this;
        }

        public VideoManifest AddPart(Guid Id, double PartLength)
        {
            manifest.AppendLine($"#EXTINF:{PartLength}");
            manifest.AppendLine($"[ENDPOINT]/[ID]/{parts++}");
            return this;
        }

        public override string ToString()
        {
            if (!headersSet)
                throw new FormatException($"Please ensure that you have call SetHeaders() method!");

            if (!finished)
            {
                manifest.AppendLine("#EXT-X-ENDLIST");
                finished = true;
            }

            return manifest.ToString();
        }

        static public VideoManifest CreateFromString(string manifestString)
        {
            var manifest = new VideoManifest();
            manifest.manifest = new StringBuilder(manifestString);
            manifest.parts = Regex.Matches(manifestString, "[ENDPOINT]").Count;
            return manifest;
        }
    }
}
