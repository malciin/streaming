using Streaming.Domain.Enums;
using System;

namespace Streaming.Domain.Models
{
    public class Video
    {
        public Guid VideoId { get; protected set; }
        public string Title { get; protected set; }
        public DateTime CreatedDate { get; protected set; }
        public DateTime? FinishedProcessingDate { get; protected set; }
        public string Description { get; protected set; }
        public VideoState State { get; protected set; }
        
        public string MainThumbnailUrl { get; protected set; }

        public TimeSpan? Length { get; protected set; }
        public VideoManifest VideoManifest { get; protected set; }

        public UserDetails Owner { get; protected set; }
        
        protected Video() { }

        public Video(Guid videoId, string title, string description, UserDetails owner)
        {
            VideoId = videoId;
            Title = title;
            Description = description;
            Owner = owner;
            CreatedDate = DateTime.UtcNow;
            State = VideoState.Fresh;
        }

        public void SetTitle(string title)
        {
            Title = title;
        }
        
        public void SetDescription(string description)
        {
            Description = description;
        }

        public void SetThumbnail(string mainThumbnailUrl)
        {
            MainThumbnailUrl = mainThumbnailUrl;
            State |= VideoState.MainThumbnailGenerated;
        }

        public void SetVideoManifest(VideoManifest manifest)
        {
            VideoManifest = manifest;
            State |= VideoState.ManifestGenerated | VideoState.Processed;
            Length = manifest.Length;
            FinishedProcessingDate = DateTime.UtcNow;
        }
    }
}
