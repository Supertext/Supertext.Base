#if NET8_0_OR_GREATER
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Supertext.Base.Exceptions;

namespace Supertext.Base.Hosting.ErrorHandling
{
    public class ProblemDetailsExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;

        public ProblemDetailsExceptionHandler(IProblemDetailsService problemDetailsService)
        {
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ProblemDetailsException problemDetailsException)
            {
                var exceptionHandlerFeature = httpContext.Features.Get<IExceptionHandlerFeature>();
                var statusCode = GetStatusCode(problemDetailsException, httpContext.Response.StatusCode == 0 ? StatusCodes.Status500InternalServerError : httpContext.Response.StatusCode);
                httpContext.Response.StatusCode = statusCode;

                return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
                                                                  {
                                                                      HttpContext = httpContext,
                                                                      AdditionalMetadata = exceptionHandlerFeature.Endpoint?.Metadata,
                                                                      ProblemDetails = { Status = statusCode },
                                                                      Exception = exception,
                                                                  });
            }

            return false;
        }

        private static int GetStatusCode(ProblemDetailsException exception, int fallbackStatusCode)
        {
            return exception switch
            {
                ForbiddenException _ => StatusCodes.Status403Forbidden,
                UnauthorizedException _ => StatusCodes.Status401Unauthorized,
                ConflictException _ => StatusCodes.Status409Conflict,
                var _ => fallbackStatusCode
            };
        }
    }
}
#endif