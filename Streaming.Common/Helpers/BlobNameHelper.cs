using System;

namespace Streaming.Common.Helpers
{
    public static class BlobNameHelper
    {
        public static string GetVideoFilename(Guid VideoId, int Part)
        {
            return $"{VideoId}_{Part}.ts";
        }

        public static string GetThumbnailFilename(Guid VideoId)
        {
            return $"{VideoId}_Thumbnail.jpg";
        }
    }
}
