using Borg.Framework.System;
using Microsoft.Extensions.Logging;

namespace Borg.Framework.Backoffice
{
    public class SystemService : ISystemService
    {
        private readonly ILoggerFactory _loggerFactory;

        public SystemService(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }



        public ILogger CreateLogger(string categoryName)
        {
            return _loggerFactory.CreateLogger(categoryName);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            _loggerFactory.AddProvider(provider);
        }

        public void Dispose()
        {
            _loggerFactory.Dispose();
        }
    }
}
