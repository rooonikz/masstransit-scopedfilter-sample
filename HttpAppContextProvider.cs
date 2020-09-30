using System;

namespace MassTransit.ScopedFilter.Sample
{
    public class BusAppContextProvider : IAppContextProvider
    {
        private readonly ConsumeContext _consumeContext;

        public BusAppContextProvider(ConsumeContext consumeContext)
        {
            _consumeContext = consumeContext;
        }

        public AppContext Get()
        {
            var tenant = _consumeContext.Headers.Get<string>(Headers.Tenant);
            var correlationId = _consumeContext.Headers.Get<string>(Headers.CorrelationId);
            var userId = _consumeContext.Headers.Get<string>(Headers.UserId);

            return new AppContext(tenant, userId, Guid.Parse(correlationId));
        }
    }

    public class NoopAppContextProvider : IAppContextProvider
    {
        public AppContext Get()
        {
            return new AppContext("notenant", Guid.Empty.ToString(), Guid.Empty);
        }
    }

    public interface IAppContextProvider
    {
        AppContext Get();
    }

    public class AppContext
    {
        public AppContext(string tenant, string userId, Guid correlationId)
        {
            Tenant = tenant;
            UserId = userId;
            CorrelationId = correlationId;
        }

        public string Tenant { get; }
        public string UserId { get; }
        public Guid CorrelationId { get; }
    }

    public static class Headers
    {
        public const string Tenant = "X-Tenant";
        public const string UserId = "X-UserId";
        public const string CorrelationId = "X-CorrelationId";
    }
}
