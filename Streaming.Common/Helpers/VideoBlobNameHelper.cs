using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Common.Helpers
{
    public static class VideoBlobNameHelper
    {
        public static string GetVideoName(Guid VideoId, int Part)
        {
            return $"{VideoId}_{Part}.ts";
        }
    }
}
