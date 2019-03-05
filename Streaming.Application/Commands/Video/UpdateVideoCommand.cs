using System;
using System.Security.Claims;

namespace Streaming.Application.Commands.Video
{
    public class UpdateVideoCommand : ICommand
    {
        public Guid VideoId { get; set; }
        public string NewTitle { get; set; }
        public string NewDescription { get; set; }
        public ClaimsPrincipal User { get; set; }
    }
}
