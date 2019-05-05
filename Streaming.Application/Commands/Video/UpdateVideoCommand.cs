using System;
using System.Security.Claims;
using Streaming.Application.Models;
using Streaming.Domain.Models;

namespace Streaming.Application.Commands.Video
{
    public class UpdateVideoCommand : IAuthenticatedCommand
    {
        public Guid VideoId { get; set; }
        public string NewTitle { get; set; }
        public string NewDescription { get; set; }
        public UserInfo User { get; set; }
    }
}
