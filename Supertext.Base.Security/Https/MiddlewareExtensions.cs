using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System;

namespace Supertext.Base.Security.Https
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Checks for the presence of <c>X-Forwarded-For</c> and <c>X-Forwarded-Proto</c> headers, and if present updates the properties of the request with those headers' details.
        /// </summary>
        /// <remarks>
        /// This extension method is needed for operating our website on an HTTP connection behind a proxy which handles SSL hand-off. Such a proxy adds the <c>X-Forwarded-For</c>
        /// and <c>X-Forwarded-Proto</c> headers to indicate the nature of the client's connection.
        /// </remarks>
        public static IApplicationBuilder EnsureHttps(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders();
            return app.Use(async (context, next) =>
                           {
                               if (context.Request.IsHttps)
                               {
                                   await next();
                               }
                               else if (context.Request.Headers["X-Forwarded-Proto"] == Uri.UriSchemeHttps)
                               {
                                   context.Request.IsHttps = true;
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