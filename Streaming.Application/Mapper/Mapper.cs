using System;
using Streaming.Application.Models.DTO.Live;
using Streaming.Application.Models.DTO.Video;
using Streaming.Domain.Models;

namespace Streaming.Application
{
    public class Mapper
    {
        public VideoMetadataDTO MapVideoMetadataDTO(Video video)
        {
            return new VideoMetadataDTO
            {
                VideoId = video.VideoId,
                CreatedDate = (DateTime)video.FinishedProcessingDate,
                Description = video.Description,
                Length = (TimeSpan)video.Length,
                Title = video.Title,
                ThumbnailUrl = video.MainThumbnailUrl,
                OwnerNickname = video.Owner.Nickname
            };
        }

        public LiveStreamMetadataDTO MapLiveStreamMetadataDTO(LiveStream liveStream)
        {
            return new LiveStreamMetadataDTO
            {
                LiveStreamId = liveStream.LiveStreamId,
                Started = liveStream.Started,
                Title = liveStream.Title,
                ManifestUrl = liveStream.ManifestUrl.AbsoluteUri,
                UserStarted = liveStream.Owner.Nickname
            };
        }

        public PastLiveStreamMetadataDTO MapPastLiveStreamMetadataDTO(LiveStream liveStream)
        {
            return new PastLiveStreamMetadataDTO
            {
                LiveStreamId = liveStream.LiveStreamId,
                Started = liveStream.Started,
                Title = liveStream.Title,
                ManifestUrl = liveStream.ManifestUrl.AbsoluteUri,
                UserStarted = liveStream.Owner.Nickname,
                Ended = liveStream.Ended
            };
        }
    }
}