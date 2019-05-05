using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Streaming.Api.Middlewares;
using Streaming.Application.SignalR.Hubs;
using Streaming.Auth0;
using Streaming.Infrastructure.IoC.Extensions;
using Streaming.Infrastructure.MongoDb.Extensions;

namespace Streaming.Api
{
    public class Startup
    {
        private readonly IStartupEvents startupEvents;
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment, IStartupEvents startupEvents)
        {
            BannerPrinter.Print(configuration, hostingEnvironment);
            this.configuration = configuration;
            this.startupEvents = startupEvents;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAny", x =>
                {
                    x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                });
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddAuth0JwtToken(authenticationDomain: configuration["Jwt:Issuer"]);

            services.AddSignalR();

            services.AddMvc().AddFluentValidation(x =>
            {
                x.RegisterValidatorsFromAssemblyContaining<Application.Validators.UploadVideoCommandValidator>();
            });

            var builder = new ContainerBuilder();

            builder.Populate(services);
            builder.UseDefaultModules();
            builder.UseMongoDb(configuration["Database:ConnectionString"]);
            startupEvents?.ConfigureServicesAutofacCallback?.Invoke(builder);

            return new AutofacServiceProvider(builder.Build());
        }

        public void Configure(IApplicationBuilder app)
        {
            startupEvents?.AppConfigurationBeginingCallback?.Invoke(app);
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<ValidationExceptionHandlerMiddleware>();
            app.UseCors("AllowAny");
            app.UseAuthentication();
            startupEvents?.AppConfigurationAfterAuthenticationCallback?.Invoke(app);
            app.UseSignalR(config =>
            {
                config.MapHub<FFmpegProcessingHub>("/hub/processingInfo");
            });
            app.UseMvc();
        }
    }
}
