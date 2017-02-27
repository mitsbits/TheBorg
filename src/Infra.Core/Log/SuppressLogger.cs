using System;
using Microsoft.Extensions.Logging;

namespace Borg.Infra.Core.Log
{
    public class SuppressLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NullScope();
        }

        public class NullScope : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}