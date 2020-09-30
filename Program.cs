using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MassTransit.ScopedFilter.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureServices((context, collection) =>
                {
                    collection.AddSingleton(new AppSettings(context.Configuration));
                    collection.AddHostedService<BusHostedService>();
                })
                .ConfigureContainer<ContainerBuilder>((context, builder) =>
                {
                    builder.RegisterModule(new Module(new AppSettings(context.Configuration)));
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>();
                });
    }
}
