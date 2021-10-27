using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Supertext.Base.Http;

namespace Supertext.Base.Hosting.Middleware
{
    internal class RequestDomainMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestDomainMiddleware> _logger;

        public RequestDomainMiddleware(RequestDelegate next, ILogger<RequestDomainMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IDomainInitializer urlResolver)
        {
            var domain = context.Request.Host.Host;

            if (!String.IsNullOrWhiteSpace(domain))
            {
                urlResolver.AddDomain(domain);
                _logger.LogInformation($"{nameof(RequestDomainMiddleware)} {nameof(domain)}={domain}");
            }

            await _next(context).ConfigureAwait(false);
        }
    }
}