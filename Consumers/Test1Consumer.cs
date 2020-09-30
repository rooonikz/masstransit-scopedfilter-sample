using System;
using System.Threading.Tasks;

namespace MassTransit.ScopedFilter.Sample.Consumers
{
    public class Test1Consumer : IConsumer<Test1>
    {
        private readonly IAppContextProvider _provider;

        public Test1Consumer(IAppContextProvider provider)
        {
            _provider = provider;
        }

        public async Task Consume(ConsumeContext<Test1> context)
        {
            Console.WriteLine($"Test1 : {_provider.Get().Tenant}");
            await context.Publish(new Test2());
        }
    }

    public class Test1
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
