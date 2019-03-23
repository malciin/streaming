using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Streaming.Api.Middlewares;
using Streaming.Api.Monitor;
using Streaming.Application.SignalR.Hubs;
using Streaming.Auth0;
using Streaming.Infrastructure.IoC.Extensions;
using Streaming.Infrastructure.MongoDb.Extensions;

namespace Streaming.Api
{
    public class Startup
    {
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
                options.AddPolicy("AllowLocalhost", x =>
                {
                    x.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                });
            });

            services.AddScoped<ICustomLogger, CustomLogger>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddAuth0JwtToken(authenticationDomain: configuration["Jwt:Issuer"]);

            services.AddSignalR();

            services.AddMvc().AddFluentValidation(x =>
            {
                x.RegisterValidatorsFromAssemblyContaining<Application.Validators.UploadVideoDTOValidator>();
            });

            var builder = new ContainerBuilder();

            builder.Populate(services);
            builder.UseDefaultModules();
            builder.UseMongoDb(configuration["Database:ConnectionString"], "streaming");

            return new AutofacServiceProvider(builder.Build());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsProduction())
            {
                app.UseMiddleware<ExceptionHandlingMiddleware>();
            }

            app.UseCors("AllowLocalhost");
            app.UseAuthentication();
            app.UseSignalR(config =>
            {
                config.MapHub<FFmpegProcessingHub>("/hub/processingInfo");
            });
            app.UseMvc();
        }
    }
}
