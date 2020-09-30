using System;
using System.Threading.Tasks;

namespace MassTransit.ScopedFilter.Sample.Consumers
{
    public class Test2Consumer : IConsumer<Test2>
    {
        private readonly IAppContextProvider _provider;

        public Test2Consumer(IAppContextProvider provider)
        {
            _provider = provider;
        }

        public Task Consume(ConsumeContext<Test2> context)
        {
            Console.WriteLine(_provider.Get().Tenant);
            return Task.CompletedTask;
        }
    }

    public class Test2
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
