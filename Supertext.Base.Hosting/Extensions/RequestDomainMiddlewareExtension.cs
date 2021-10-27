using Microsoft.AspNetCore.Builder;
using Supertext.Base.Hosting.Middleware;

namespace Supertext.Base.Hosting.Extensions
{
    public static class RequestDomainMiddlewareExtension
    {
        public static IApplicationBuilder UseDomainResolver(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestDomainMiddleware>();
        }
    }
}