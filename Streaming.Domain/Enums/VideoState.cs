using System;

namespace Streaming.Domain.Enums
{
    [Flags]
    public enum VideoState
    {
        Fresh = 0,
        Processed = 1<<0,
        ManifestGenerated = 1<<1,
        MainThumbnailGenerated = 1<<2,
        OverviewThumbnailsGenerated = 1<<3
    }
}
