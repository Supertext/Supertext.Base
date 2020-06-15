using Microsoft.AspNetCore.Builder;
using Supertext.Base.Hosting.Middleware;

namespace Supertext.Base.Hosting.Extensions
{
    public static class ExceptionHandlingMiddlewareExtensions
    {
        /// <summary>
        /// Uses  <see cref="ExceptionHandlingMiddleware"/> to handle uncaught exceptions. Those exceptions are logged as critical.
        /// In API controllers an <see cref="Supertext.Base.Net.Http.HttpException"/> with detailed message can be thrown, which in turn will be caught and handled by the <see cref="ExceptionHandlingMiddleware"/>.
        ///
        /// Module <see cref="Supertext.Base.Conversion.ConversionModule"/> must be registered.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}