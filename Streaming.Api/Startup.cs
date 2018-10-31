using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Streaming.Api.Middlewares;
using Streaming.Api.Monitor;

namespace Streaming.Api
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICustomLogger, CustomLogger>();
            services.AddAutoMapper();
            services.AddMvc();

            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.RegisterModule<Application.Commands._CommandModule>();
            builder.RegisterModule<Application.Persistence.MongoModule>();
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
