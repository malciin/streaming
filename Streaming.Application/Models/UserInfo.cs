using System.Linq;
using Streaming.Domain.Models;

namespace Streaming.Application.Models
{
    public class UserInfo : UserDetails
    {
        public string[] Claims { get; set; }

        public bool HaveClaim(string claim)
            => Claims.Contains(claim);
    }
}