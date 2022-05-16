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

        public async Task InvokeAsync(HttpContext context, IHostInitializer urlResolver)
        {
            var host = context.Request.Host.Host;
           
            if (!String.IsNullOrWhiteSpace(host))
            {
                urlResolver.AddHost(host);
                _logger.LogInformation($"{nameof(RequestDomainMiddleware)} {nameof(host)}={host}");
            }

            await _next(context).ConfigureAwait(false);
        }
    }
}