﻿using Streaming.Application.DTO.Video;
using Streaming.Application.Services;
using Streaming.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Mappings
{
    public class VideoMappingService : IMappingService
    {
        private readonly IThumbnailService thumbnailService;

        public VideoMappingService(IThumbnailService thumbnailService)
        {
            this.thumbnailService = thumbnailService;
        }

        public VideoMetadataDTO MapVideoMetadataDTO(Video Video)
        {
            return new VideoMetadataDTO
            {
                VideoId = Video.VideoId,
                CreatedDate = (DateTime)Video.FinishedProcessingDate,
                Description = Video.Description,
                Length = (TimeSpan)Video.Length,
                Title = Video.Title,
                ThumbnailUrl = Video.State.HasFlag(VideoState.MainThumbnailGenerated) ?
                    thumbnailService.GetThumbnailUrl(Video.VideoId) : thumbnailService.GetPlaceholderThumbnailUrl()
            };
        }
    }
}
