using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Supertext.Base.Conversion.Json;
using Supertext.Base.Net.Http;

namespace Supertext.Base.Hosting.Middleware
{
    internal class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJsonConverter _jsonConverter;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, IJsonConverter jsonConverter, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _jsonConverter = jsonConverter;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, $"An unexpected exception occurred and has been caught by {nameof(ExceptionHandlingMiddleware)}.");

                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var errorResponse = new ErrorResponse();

            if (exception is HttpException httpException)
            {
                errorResponse.StatusCode = httpException.StatusCode;
                errorResponse.Message = httpException.Message;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)errorResponse.StatusCode;
            await context.Response.WriteAsync(_jsonConverter.Serialize(errorResponse));
        }
    }
}