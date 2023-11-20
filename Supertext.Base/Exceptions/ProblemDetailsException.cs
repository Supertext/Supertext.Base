using System;

namespace Supertext.Base.Exceptions
{
    public class ProblemDetailsException : Exception
    {
        public string ErrorKey { get; }

        public object ErrorData { get; }

        public ProblemDetailsException(string errorKey,
                                       string message,
                                       object errorData = null) : base(message)
        {
            ErrorKey = errorKey;
            ErrorData = errorData;
        }

        public ProblemDetailsException(string errorKey,
                                       string message,
                                       Exception innerException,
                                       object errorData = null) : base(message, innerException)
        {
            ErrorKey = errorKey;
            ErrorData = errorData;
        }
    }
}