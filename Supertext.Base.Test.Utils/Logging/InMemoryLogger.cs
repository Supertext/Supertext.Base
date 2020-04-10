using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Supertext.Base.Test.Utils.Logging
{
    public class InMemoryLogger : ILogger
    {
        public IList<Exception> Exceptions { get; } = new List<Exception>();
        public IList<string> Messages { get; } = new List<string>();

        public void Log<TState>(LogLevel logLevel,
                                EventId eventId,
                                TState state,
                                Exception exception,
                                Func<TState, Exception, string> formatter)
        {
            if (logLevel == LogLevel.Error)
            {
                Exceptions.Add(exception);
            }
            Messages.Add(formatter(state, exception));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}