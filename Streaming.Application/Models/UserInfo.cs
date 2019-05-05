using Streaming.Domain.Models;

namespace Streaming.Application.Models
{
    public class UserInfo
    {
        public UserDetails Details { get; set; }
        public string[] Claims { get; set; }
    }
}