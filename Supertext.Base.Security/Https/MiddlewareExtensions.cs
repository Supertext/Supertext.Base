using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System;

namespace Supertext.Base.Security.Https
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder EnsureHttps(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders();
            return app.Use(async (context, next) =>
                           {
                               if (context.Request.IsHttps || context.Request.Headers["X-Forwarded-Proto"] == Uri.UriSchemeHttps)
                               {
                                   await next();
                               }
                               else
                               {
                                   if (!(app.ApplicationServices.GetService(typeof(ILoggerFactory)) is ILoggerFactory loggerFactory))
                                   {
                                       throw new ArgumentNullException(nameof(loggerFactory));
                                   }

                                   var logger = loggerFactory.CreateLogger(nameof(MiddlewareExtensions));
                                   var clientIp = context.Request.Headers.ContainsKey("X-Forwarded-For") ? context.Request.Headers["X-Forwarded-For"].ToString() : "";
                                   logger.LogWarning($"An HTTP (non-SSL) request was made. Path: {context.Request.Path}, client IP: {clientIp}");

                                   var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : String.Empty;
                                   var https = "https://" + context.Request.Host + context.Request.Path + queryString;
                                   context.Response.Redirect(https, true);
                               }
                           });
        }
    }
}