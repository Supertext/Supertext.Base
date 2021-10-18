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
        private readonly IDomainInitializer _urlResolver;

        public RequestDomainMiddleware(RequestDelegate next, IDomainInitializer urlResolver, ILogger<RequestDomainMiddleware> logger)
        {
            _next = next;
            _urlResolver = urlResolver;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var domain = context.Request.Host.Host;

            if (!String.IsNullOrWhiteSpace(domain))
            {
                _urlResolver.AddDomain(domain);
                _logger.LogInformation($"{nameof(RequestDomainMiddleware)} {nameof(domain)}={domain}");
            }

            await _next(context).ConfigureAwait(false);
        }
    }
}