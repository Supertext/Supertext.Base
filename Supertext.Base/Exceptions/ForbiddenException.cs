using System;

namespace Supertext.Base.Exceptions
{
    public class UnauthorizedException : ProblemDetailsException
    {
        public UnauthorizedException(string errorKey, string message, object errorData = null) : base(errorKey,
                                                                                                      message,
                                                                                                      errorData)
        {
        }

        public UnauthorizedException(string errorKey,
                                     string message,
                                     Exception innerException,
                                     object errorData = null) : base(errorKey,
                                                                     message,
                                                                     innerException,
                                                                     errorData)
        {
        }
    }
}