using System;

namespace Supertext.Base.Exceptions
{
    public class ConflictException : ProblemDetailsException
    {
        public ConflictException(string errorType, string message, object errorData = null) : base(errorType,
                                                                                                   message,
                                                                                                   errorData)
        {
        }

        public ConflictException(string errorType,
                                 string message,
                                 Exception innerException,
                                 object errorData = null) : base(errorType,
                                                                 message,
                                                                 innerException,
                                                                 errorData)
        {
        }
    }
}