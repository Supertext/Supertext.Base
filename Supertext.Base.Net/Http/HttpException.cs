using System;
using System.Net;

namespace Supertext.Base.Net.Http
{
    /// <summary>
    /// Can be used to throw exceptions in API controllers. It will be caught and handled by an exception middleware.
    /// The middleware can be configured by using <see cref="Supertext.Base.Hosting.Extensions.ExceptionHandlingMiddlewareExtensions"/>
    /// </summary>
    public class HttpException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public HttpException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}