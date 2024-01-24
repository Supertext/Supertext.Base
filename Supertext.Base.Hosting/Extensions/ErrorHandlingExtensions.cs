#if NET8_0_OR_GREATER
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Supertext.Base.Exceptions;
using Supertext.Base.Hosting.ErrorHandling;

namespace Supertext.Base.Hosting.Extensions
{
    public static class ErrorHandlingExtensions
    {
        private const string UnknownErrorOccurred = "Unknown error occurred";
        private const string GeneralErrorKey = "general";
        private const string ErrorKeyExtensionKey = "errorKey";
        private const string MessageExtensionKey = "message";
        private const string DataExtensionKey = "data";

        public static IServiceCollection AddProblemDetailsExceptionHandling(this IServiceCollection services)
        {
            return services
                   .AddProblemDetails(options =>
                                      {
                                          options.CustomizeProblemDetails = detailsContext =>
                                                                            {
                                                                                if (detailsContext.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error is ProblemDetailsException exception)
                                                                                {
                                                                                    detailsContext.ProblemDetails.Extensions.Add(ErrorKeyExtensionKey, exception.ErrorKey);
                                                                                    detailsContext.ProblemDetails.Extensions.Add(MessageExtensionKey, exception.Message);
                                                                                    detailsContext.ProblemDetails.Extensions.Add(DataExtensionKey, exception.Data);
                                                                                }
                                                                                else
                                                                                {
                                                                                    detailsContext.ProblemDetails.Extensions.Add(ErrorKeyExtensionKey, GeneralErrorKey);
                                                                                    detailsContext.ProblemDetails.Extensions.Add(MessageExtensionKey, UnknownErrorOccurred);
                                                                                }
                                                                            };
                                      })
                   .AddExceptionHandler<ProblemDetailsExceptionHandler>();
        }
    }
}
#endif