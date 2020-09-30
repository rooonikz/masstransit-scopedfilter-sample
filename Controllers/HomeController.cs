using System;
using System.Threading.Tasks;
using MassTransit.ScopedFilter.Sample.Consumers;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.ScopedFilter.Sample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IBus _bus;

        public HomeController(IBus bus)
        {
            _bus = bus;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Data data)
        {
            await _bus.Publish(new Test1(), context =>
            {
                context.SetTenant(data.Tenant);
                context.SetCorrelationId(Guid.NewGuid());
            });
            await _bus.Publish(new Test1(), context =>
            {
                context.SetTenant("tenant2");
                context.SetCorrelationId(Guid.NewGuid());
            });
            return Ok();
        }

        public class Data
        {
            public string Tenant { get; set; }
        }
    }
}
