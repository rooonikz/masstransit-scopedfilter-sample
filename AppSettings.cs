using System;
using Microsoft.Extensions.Configuration;

namespace MassTransit.ScopedFilter.Sample
{
    public class AppSettings
    {
        public AppSettings(IConfiguration configuration)
        {
            configuration.Bind(this);
        }

        public BusConfigSettings BusConfig { get; set; } = new BusConfigSettings();
        public RabbitMqBusConfig RabbitMqBus { get; set; } = new RabbitMqBusConfig();
        public AzureBusConfig AzureBus { get; set; } = new AzureBusConfig();

        public class RabbitMqBusConfig
        {
            public string HostUri { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class AzureBusConfig
        {
            public string HostUri { get; set; }
            public string KeyName { get; set; }
            public string PrimaryKey { get; set; }
            public TimeSpan TokenTimeToLive { get; set; } = TimeSpan.FromDays(1);
        }

        public class BusConfigSettings
        {
            public string BusTransport { get; set; }
        }
    }
}
