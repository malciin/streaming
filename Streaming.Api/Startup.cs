using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Streaming.Api.Configurations;
using Streaming.Api.Middlewares;
using Streaming.Api.Monitor;
using Streaming.Application.Configuration;

namespace Streaming.Api
{
    public class Startup
    {
        
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDirectoriesConfiguration, DirectoriesConfiguration>(x => new DirectoriesConfiguration((IConfigurationRoot)x.GetRequiredService<IConfiguration>()));
            services.AddSingleton<IKeysConfiguration, KeysConfiguration>(x => new KeysConfiguration((IConfigurationRoot)x.GetRequiredService<IConfiguration>()));
            services.AddScoped<ICustomLogger, CustomLogger>();
            services.AddAutoMapper();
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
            builder.Populate(services);

            builder.RegisterModule<Application.Commands._CommandModule>();
            builder.RegisterModule<Application.Services._ServicesModule>();

            return new AutofacServiceProvider(builder.Build());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseMiddleware<ExceptionHandlingMiddleware>();
            }

            app.UseMvc();
        }
    }
}
