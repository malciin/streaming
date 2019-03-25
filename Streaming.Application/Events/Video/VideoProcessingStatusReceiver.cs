using Microsoft.AspNetCore.SignalR;
using Streaming.Application.Models.DTO.Video;
using Streaming.Application.SignalR.Hubs;
using System;
using System.Threading.Tasks;

namespace Streaming.Application.Events.Video
{
    public class VideoProcessingStatusReceiver : IEventReceiver<VideoProcessingStatusEvent>
    {
        IHubContext<FFmpegProcessingHub> ffmpegHubContext;
        public VideoProcessingStatusReceiver(IHubContext<FFmpegProcessingHub> ffmpegHubContext)
        {
            this.ffmpegHubContext = ffmpegHubContext;
        }

        public async Task Receive(VideoProcessingStatusEvent @event)
        {
            var connectionId = FFmpegProcessingHub.Informations.GetConnectionId(@event.UserId);
            if (!String.IsNullOrEmpty(connectionId))
                await ffmpegHubContext.Clients.Client(connectionId).SendAsync("ProcessingInformation", @event.Output);
        }
    }
}
