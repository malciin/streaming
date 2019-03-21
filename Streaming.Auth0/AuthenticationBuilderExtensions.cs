using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace Streaming.Auth0
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddAuth0JwtToken(this AuthenticationBuilder builder, string authenticationDomain, string validAudience = null)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = !String.IsNullOrEmpty(validAudience),

                ValidIssuer = authenticationDomain,
                ValidAudience = validAudience,

                IssuerSigningKey = RsaSecurityKeyHelper.GetSecurityKey(authenticationDomain, localFileName: "auth0_jwsk.json"),
                ValidateIssuerSigningKey = true,
                ValidateActor = false,
                ValidateLifetime = false,
                ValidateTokenReplay = false
            };


            return builder.AddJwtBearer(x =>
            {
                x.TokenValidationParameters = tokenValidationParameters;
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
