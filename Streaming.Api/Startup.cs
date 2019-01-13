using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Streaming.Api.Middlewares;
using Streaming.Api.Monitor;

namespace Streaming.Api
{
    public class Startup
    {
        
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

            services.AddMvc().AddFluentValidation(x =>
            {
                x.RegisterValidatorsFromAssemblyContaining<Application.Validators.VideoUploadValidator>();
            });
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
            });

            var builder = new ContainerBuilder();

            Application.MongoDb.Mappings.Map();

            builder.Populate(services);

            builder.RegisterModule<Application.Modules.CommandModule>();
            builder.RegisterModule<Application.Modules.ServicesModule>();
            builder.RegisterModule<Application.Modules.SettingsModule>();
			builder.RegisterModule<Application.Modules.QueryModule>();
            builder.RegisterModule<Application.Modules.StrategiesModule>();

            return new AutofacServiceProvider(builder.Build());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseMiddleware<ExceptionHandlingMiddleware>();
            }

            app.UseCors("AllowAll");
            app.UseMvc();
        }
    }
}
