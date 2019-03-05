using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Streaming.Api.Middlewares;
using Streaming.Api.Monitor;
using Streaming.Auth0;
using Streaming.IoC.Extensions;

namespace Streaming.Api
{
    

    public class Startup
    {
        private SecurityKey GetAuth0RsaSecurityKey()
        {
            JObject jwks;
            var jwksFileName = "jwks.json";
            if (!File.Exists(jwksFileName))
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile($"https://id0.eu.auth0.com/.well-known/jwks.json", jwksFileName);
                }
            }
            jwks = JObject.Parse(jwksFileName);

            var parameters = new RSAParameters();
            parameters.Exponent = Base64UrlEncoder.DecodeBytes(jwks["e"].ToString());
            parameters.Modulus = Base64UrlEncoder.DecodeBytes(jwks["n"].ToString());
            return new RsaSecurityKey(parameters);
        }

        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IConfiguration configuration;

        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
        }


        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", x =>
                {
                    x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddScoped<ICustomLogger, CustomLogger>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddAuth0JwtToken(authenticationDomain: configuration["Jwt:Issuer"]);

            services.AddMvc().AddFluentValidation(x =>
            {
                x.RegisterValidatorsFromAssemblyContaining<Application.Validators.UploadVideoDTOValidator>();
            });

            var builder = new ContainerBuilder();

            Application.MongoDb.Mappings.Map();

            builder.Populate(services);
            builder.UseDefaultModules();

            return new AutofacServiceProvider(builder.Build());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsProduction())
            {
                app.UseMiddleware<ExceptionHandlingMiddleware>();
            }

            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
