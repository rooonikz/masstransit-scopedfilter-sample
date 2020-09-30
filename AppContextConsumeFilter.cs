using System.Threading.Tasks;
using GreenPipes;
using Microsoft.Extensions.Logging;

namespace MassTransit.ScopedFilter.Sample
{
    public class AppContextConsumeFilter<TMessage> : IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        private readonly IAppContextProvider _appContextProvider;
        private readonly ILogger<AppContextConsumeFilter<TMessage>> _logger;

        public AppContextConsumeFilter(IAppContextProvider appContextProvider, ILogger<AppContextConsumeFilter<TMessage>> logger)
        {
            _appContextProvider = appContextProvider;
            _logger = logger;
        }

        public Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            var appContext = _appContextProvider.Get();

            using var scope = _logger.BeginScope(
                "Tenant: {AppContext.Tenant}, CorrelationID: {AppContext.CorrelationId}",
                appContext.Tenant,
                appContext.CorrelationId);

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("appContextConsumeScope");
        }
    }
}
