using Borg.Framework.System;
using Microsoft.Extensions.Logging;

namespace Borg.Framework.Backoffice
{
    public class SystemService : ISystemService<BorgSettings>
    {
        private readonly ILoggerFactory _loggerFactory;

        public BorgSettings Settings { get; }

        public SystemService(ILoggerFactory loggerFactory, BorgSettings settings)
        {
            _loggerFactory = loggerFactory;
            Settings = settings;
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