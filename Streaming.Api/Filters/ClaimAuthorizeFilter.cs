using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using Streaming.Common.Extensions;

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
            if (!context.HttpContext.User.HasAnyStreamingClaim(claims))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
