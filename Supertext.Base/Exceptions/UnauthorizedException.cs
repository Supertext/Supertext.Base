using System;

namespace Supertext.Base.Exceptions
{
    public class ForbiddenException : ProblemDetailsException
    {
        public ForbiddenException(string errorKey, string message, object errorData = null) : base(errorKey,
                                                                                                   message,
                                                                                                   errorData)
        {
        }

        public ForbiddenException(string errorKey,
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