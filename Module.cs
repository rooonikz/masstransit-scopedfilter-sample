using Autofac;

namespace MassTransit.ScopedFilter.Sample
{
    internal class Module : Autofac.Module
    {
        private readonly AppSettings _appSettings;

        public Module(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Register MongoDb instances per tenant
            base.Load(builder);

            builder
                .Register<IAppContextProvider>(context =>
                {
                    if (context.TryResolve(out ConsumeContext consumerContext))
                    {
                        return new BusAppContextProvider(consumerContext);
                    }

                    return new NoopAppContextProvider();
                })
                .As<IAppContextProvider>()
                .InstancePerLifetimeScope();

            builder.AddMassTransit(configurator =>
            {
                configurator.AddConsumers(ThisAssembly);
                configurator.ConfigureBus(_appSettings.RabbitMqBus, _appSettings.AzureBus, _appSettings.BusConfig);
            });
        }
    }
}
