#if NET8_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Supertext.Base.Hosting.ErrorHandling;

namespace Supertext.Base.Hosting.Extensions
{
    public static class ErrorHandlingExtensions
    {
        public static IServiceCollection AddProblemDetailsExceptionHandling(this IServiceCollection services)
        {
            return services.AddTransient<IProblemDetailsWriter, ProblemDetailsExceptionWriter>()
                           .AddProblemDetails()
                           .AddExceptionHandler<ProblemDetailsExceptionHandler>();
        }
    }
}
#endif