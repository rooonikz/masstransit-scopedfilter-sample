using System;
using System.Threading.Tasks;
using GreenPipes;

namespace MassTransit.ScopedFilter.Sample
{
    public class AppContextEnricherFilter<TMessage> : IFilter<SendContext<TMessage>>, IFilter<PublishContext<TMessage>>
           where TMessage : class
    {
        private readonly IAppContextProvider _appContextProvider;
        private readonly IServiceProvider _serviceProvider;

        public AppContextEnricherFilter(IAppContextProvider appContextProvider, IServiceProvider serviceProvider)
        {
            _appContextProvider = appContextProvider;
            _serviceProvider = serviceProvider;
        }

        public Task Send(SendContext<TMessage> context, IPipe<SendContext<TMessage>> next)
        {
            SetHeaders(context);
            return next.Send(context);
        }

        public Task Send(PublishContext<TMessage> context, IPipe<PublishContext<TMessage>> next)
        {
            SetHeaders(context);
            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("appContextEnrichScope");
        }

        private void SetHeaders(SendContext context)
        {
            var appContext = _appContextProvider.Get();

            var tenant = appContext.Tenant;
            if (!string.IsNullOrWhiteSpace(tenant)
                && !context.Headers.TryGetHeader(Headers.Tenant, out _))
            {
                context.SetTenant(tenant);
            }

            var userId = appContext.UserId;
            if (!string.IsNullOrWhiteSpace(userId)
                && !context.Headers.TryGetHeader(Headers.UserId, out _))
            {
                context.SetUserId(userId);
            }

            var correlationId = appContext.CorrelationId;
            if (correlationId != Guid.Empty
                && !context.Headers.TryGetHeader(Headers.CorrelationId, out _))
            {
                context.SetCorrelationId(correlationId);
            }
        }
    }

    public static class SendContextExtensions
    {
        public static SendContext SetTenant(this SendContext ctx, string tenant)
        {
            ctx.Headers.Set(Headers.Tenant, tenant);
            return ctx;
        }

        public static SendContext SetCorrelationId(this SendContext ctx, Guid correlationId)
        {
            ctx.Headers.Set(Headers.CorrelationId, correlationId.ToString());
            return ctx;
        }

        public static SendContext SetUserId(this SendContext ctx, string userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                ctx.Headers.Set(Headers.UserId, userId);
            }

            return ctx;
        }
    }
}
