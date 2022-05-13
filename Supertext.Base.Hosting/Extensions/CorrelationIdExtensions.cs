using System;
using Microsoft.AspNetCore.Builder;
using Supertext.Base.Hosting.Middleware;

namespace Supertext.Base.Hosting.Extensions;

public static class CorrelationIdExtensions
{
    /// <summary>
    /// Use this for having correlation id logged as and having an injectable ITracingProvider.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
}