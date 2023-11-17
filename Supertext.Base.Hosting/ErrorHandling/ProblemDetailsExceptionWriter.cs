#if NET8_0_OR_GREATER
using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Supertext.Base.Exceptions;

namespace Supertext.Base.Hosting.ErrorHandling
{
    internal class ProblemDetailsExceptionWriter : IProblemDetailsWriter
    {
        private const string UnknownErrorOccurred = "Unknown error occurred";
        private const string GeneralErrorKey = "general";

        private static readonly MediaTypeCollection ProblemContentTypes = new()
                                                                          {
                                                                              "application/problem+json",
                                                                              "application/problem+xml"
                                                                          };

        private readonly OutputFormatterSelector _formatterSelector;
        private readonly IHttpResponseStreamWriterFactory _writerFactory;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public ProblemDetailsExceptionWriter(OutputFormatterSelector formatterSelector,
                                             IHttpResponseStreamWriterFactory writerFactory,
                                             ProblemDetailsFactory problemDetailsFactory)
        {
            _formatterSelector = formatterSelector;
            _writerFactory = writerFactory;
            _problemDetailsFactory = problemDetailsFactory;
        }

        public bool CanWrite(ProblemDetailsContext context)
        {
            var exceptionHandlerFeature = context.HttpContext.Features.Get<IExceptionHandlerFeature>();

            return exceptionHandlerFeature?.Error is ProblemDetailsException;
        }

        public ValueTask WriteAsync(ProblemDetailsContext context)
        {
            var problemDetails = CreateNewProblemDetails(context);

            ExtendProblemDetails(context, problemDetails);

            var formatterContext = new OutputFormatterWriteContext(context.HttpContext,
                                                                   _writerFactory.CreateWriter,
                                                                   typeof(ProblemDetails),
                                                                   problemDetails);

            var formatter = _formatterSelector.SelectFormatter(formatterContext,
                                                               Array.Empty<IOutputFormatter>(),
                                                               ProblemContentTypes);

            return formatter == null ? ValueTask.CompletedTask : new ValueTask(formatter.WriteAsync(formatterContext));
        }

        private ProblemDetails CreateNewProblemDetails(ProblemDetailsContext context)
        {
            var problemDetails = _problemDetailsFactory.CreateProblemDetails(context.HttpContext,
                                                                             context.ProblemDetails.Status ?? context.HttpContext.Response.StatusCode,
                                                                             context.ProblemDetails.Title,
                                                                             context.ProblemDetails.Type,
                                                                             context.ProblemDetails.Detail,
                                                                             context.ProblemDetails.Instance);
            foreach (var extension in context.ProblemDetails.Extensions)
            {
                problemDetails.Extensions[extension.Key] = extension.Value;
            }

            return problemDetails;
        }

        private static void ExtendProblemDetails(ProblemDetailsContext context, ProblemDetails problemDetails)
        {
            if (context.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error is ProblemDetailsException exception)
            {
                problemDetails.Extensions.Add("errorKey", exception.ErrorKey);
                problemDetails.Extensions.Add("message", exception.Message);
                problemDetails.Extensions.Add("data", exception.Data);
            }
            else
            {
                problemDetails.Extensions.Add("errorKey", GeneralErrorKey);
                problemDetails.Extensions.Add("message", UnknownErrorOccurred);
            }
        }
    }
}
#endif