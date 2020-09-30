
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MassTransit.ScopedFilter.Sample
{
    internal class BusHostedService : IHostedService
    {
        private readonly IBusControl _bus;
        private readonly ILogger<BusHostedService> _logger;

        public BusHostedService(
            IBusControl bus,
            ILogger<BusHostedService> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bus.StartAsync(cancellationToken);
            _logger.LogInformation("Service {app}: bus started");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _bus.StopAsync(cancellationToken);
            _logger.LogInformation("Service {app}: bus stopped");
        }
    }
}
