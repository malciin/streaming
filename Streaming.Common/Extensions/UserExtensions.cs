using System;
using System.Linq;

namespace Streaming.Common.Extensions
{
    public static class UserExtensions
    {
        private static readonly string streamingClaimType = "http://streaming.com/claims";
        public static bool HasStreamingClaim(this System.Security.Claims.ClaimsPrincipal user, string claimName)
            => user.HasClaim(x => x.Type == streamingClaimType && x.Value == claimName);

        public static bool HasAnyStreamingClaim(this System.Security.Claims.ClaimsPrincipal user, params string[] claims)
            => user?.Claims.Where(x => x.Type == streamingClaimType)
                .Any(x => Array.Exists(claims, y => y.Equals(x.Value))) ?? false;

    }
}
