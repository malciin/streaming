using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Streaming.Api.Attributes
{
    public class ClaimAuthorizeAttribute : TypeFilterAttribute
    {
        public ClaimAuthorizeAttribute(params string[] claims) : base(typeof(ClaimAuthorizeFilter))
        {
            Arguments = new object[] { claims };
        }
    }

    public class ClaimAuthorizeFilter : IAuthorizationFilter
    {
        private readonly string[] claims;
        public ClaimAuthorizeFilter(params string[] claims)
        {
            this.claims = claims;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasNeededClaim = context.HttpContext.User.Claims.Where(x => x.Type == "http://streaming.com/claims")
                .Any(x => Array.Exists(claims, y => y.Equals(x.Value)));
            if (!hasNeededClaim)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
