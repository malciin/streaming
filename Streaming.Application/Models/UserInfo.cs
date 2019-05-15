using System.Linq;
using Streaming.Domain.Models;

namespace Streaming.Application.Models
{
    public class UserInfo
    {
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        
        public string[] Claims { get; set; }

        public bool HaveClaim(string claim)
            => Claims.Contains(claim);
    }
}