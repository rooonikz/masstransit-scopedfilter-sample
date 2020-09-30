using System;
using System.Collections.Generic;
using GreenPipes;
using MassTransit.AutofacIntegration;
using MassTransit.ScopedFilter.Sample.Consumers;

namespace MassTransit.ScopedFilter.Sample
{
    internal static class BusConfigurator
    {
        private static Action<IBusRegistrationContext, IBusFactoryConfigurator, AppSettings.BusConfigSettings>
            BusConfiguration
        {
            get
            {
                return (registrationContext, busFactoryCfg, busConfigSettings) =>
                {
                    var endpointConfigurators = new Dictionary<string, Action<IReceiveEndpointConfigurator>>
                    {
                        ["MassTransit.Sample"] = cfg =>
                        {
                            cfg.ConfigureConsumer<Test1Consumer>(registrationContext);
                            cfg.ConfigureConsumer<Test2Consumer>(registrationContext);

                            //if (cfg is IServiceBusEndpointConfigurator azureConfigurator)
                            //{
                            //    azureConfigurator.ConfigureAzure();
                            //}

                            cfg.UseRetry(r =>
                                r.Intervals(
                                    TimeSpan.FromSeconds(0.1),
                                    TimeSpan.FromSeconds(0.3),
                                    TimeSpan.FromSeconds(1.0),
                                    TimeSpan.FromSeconds(1.5),
                                    TimeSpan.FromSeconds(2)));
                            cfg.UseInMemoryOutbox();
                        }
                    };

                    busFactoryCfg.ConfigureJsonSerializer(settings =>
                    {
                        settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                        return settings;
                    });

                    busFactoryCfg.UseConsumeFilter(typeof(AppContextConsumeFilter<>), registrationContext);

                    foreach (var (queueName, configure) in endpointConfigurators)
                    {
                        busFactoryCfg.ReceiveEndpoint(queueName, configure);
                    }

                    //busFactoryCfg.UseSendFilter(typeof(AppContextEnricherFilter<>), registrationContext);
                    busFactoryCfg.UsePublishFilter(typeof(AppContextEnricherFilter<>), registrationContext);
                };
            }
        }

        public static void ConfigureBus(
            this IContainerBuilderBusConfigurator configurator,
            AppSettings.RabbitMqBusConfig rabbitMqBusConfig,
            AppSettings.AzureBusConfig azureBusConfig,
            AppSettings.BusConfigSettings busConfigSettings)
        {
            switch (busConfigSettings.BusTransport.ToLower())
            {
                //case "azure":
                //    configurator.UsingAzureServiceBus((context, cfg) =>
                //    {
                //        cfg.Host(new Uri(azureBusConfig.HostUri), host =>
                //        {
                //            host.TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                //                azureBusConfig.KeyName,
                //                azureBusConfig.PrimaryKey,
                //                TimeSpan.FromDays(1),
                //                TokenScope.Namespace);
                //        });
                //        BusConfiguration(context, cfg, busConfigSettings);
                //    });
                //    break;

                case "rabbitmq":
                    configurator.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(new Uri(rabbitMqBusConfig.HostUri), host =>
                        {
                            host.Username(rabbitMqBusConfig.Username);
                            host.Password(rabbitMqBusConfig.Password);
                        });
                        BusConfiguration(context, cfg, busConfigSettings);
                    });
                    break;

                default:
                    throw new NotSupportedException($"Bus transport {busConfigSettings.BusTransport} is not supported");
            }
        }
    }
}
