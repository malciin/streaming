﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
    public interface IThumbnailService
    {
        string GetThumbnailUrl(Guid VideoId);
        string GetPlaceholderThumbnailUrl();
        Task<Stream> GetThumbnailAsync(Guid VideoId);
        Task UploadAsync(Guid VideoId, Stream Stream);
    }
}
