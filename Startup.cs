using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Autofac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace MassTransit.ScopedFilter.Sample
{
    public class Startup
    {
        private readonly AppSettings _appSettings;

        public Startup(IConfiguration configuration)
        {
            _appSettings = new AppSettings(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpsRedirection(_ => { });
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.MaxAge = TimeSpan.FromDays(365);
                options.IncludeSubDomains = true;
            });

            services.AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                options.CustomSchemaIds(t => t.FullName);
            });

            services.AddHeaderPropagation(options =>
            {
                options.Headers.Add("x-correlation-id");
            });

            services.AddHostedService<BusHostedService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRewriter(new RewriteOptions()
                .AddRedirect("^$", "swagger", StatusCodes.Status301MovedPermanently));

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                options.DisplayRequestDuration();
            });

            app.UseHttpsRedirection();
            app.UseHsts();
            app.UseCors("Default");
            //app.UseCorrelation();
            app.UseHeaderPropagation();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseMiddleware<AppContextMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new Module(_appSettings));
        }
    }
}
