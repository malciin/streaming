using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Streaming.Api.SignalR.Hubs
{
    [Route("signalr/ffmpeg-processing-hub")]
    public class FFmpegProcessingHub : Hub
    {
    }
}
