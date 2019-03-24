using Streaming.Application.Models;
using System;
using System.Linq;

namespace Streaming.Application.Extensions
{
    public static class UserExtensions
    {
        public static bool HasStreamingClaim(this System.Security.Claims.ClaimsPrincipal user, string claimName)
            => user.HasClaim(x => x.Type == Claims.ClaimsNamespace && x.Value == claimName);

        public static bool HasAnyStreamingClaim(this System.Security.Claims.ClaimsPrincipal user, params string[] claims)
            => user?.Claims.Where(x => x.Type == Claims.ClaimsNamespace)
                .Any(x => Array.Exists(claims, y => y.Equals(x.Value))) ?? false;

    }
}
