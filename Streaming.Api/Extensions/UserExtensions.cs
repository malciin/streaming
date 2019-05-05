using Streaming.Application.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace Streaming.Api.Extensions
{
    public static class UserExtensions
    {
        public static string[] GetStreamingClaims(this ClaimsPrincipal user)
            => user.Claims.Where(x => x.Type == Claims.ClaimsNamespace).Select(x => x.Value).ToArray();

        public static bool HasStreamingClaim(this ClaimsPrincipal user, string claimName)
            => user.HasClaim(x => x.Type == Claims.ClaimsNamespace && x.Value == claimName);

        public static bool HasAnyStreamingClaim(this ClaimsPrincipal user, params string[] claims)
            => user?.Claims.Where(x => x.Type == Claims.ClaimsNamespace)
                .Any(x => Array.Exists(claims, y => y.Equals(x.Value))) ?? false;

    }
}
