using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Supertext.Base.Tracing;

namespace Supertext.Base.Hosting.Specs.Middleware.Infrastructure
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ITracingProvider _tracingProvider;
        private readonly ILogger<TestController> _logger;

        public TestController(ITracingProvider tracingProvider, ILogger<TestController> logger)
        {
            _tracingProvider = tracingProvider;
            _logger = logger;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<string> Get(int id)
        {
            _logger.LogInformation($"{nameof(Get)} - Invoked with id={id}; Correlation id={_tracingProvider.CorrelationId}");

            return await Task.FromResult(_tracingProvider.CorrelationId.ToString());
        }
    }
}