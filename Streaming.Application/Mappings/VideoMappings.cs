using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Video;
using Streaming.Domain.Enums;
using Streaming.Domain.Models;
using System;

namespace Streaming.Application.Mappings
{
    public class VideoMappings
    {
        private readonly IThumbnailService thumbnailService;

        public VideoMappings(IThumbnailService thumbnailService)
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
                ThumbnailUrl = Video.State.HasFlag(VideoState.MainThumbnailGenerated) ? thumbnailService.GetThumbnailUrl(Video.VideoId) : null,
                Owner = Video.Owner
            };
        }
    }
}
