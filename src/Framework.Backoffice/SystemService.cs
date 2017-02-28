using System;
using System.Threading.Tasks;
using Borg.Framework.System;
using Borg.Infra;
using Microsoft.Extensions.Logging;

namespace Borg.Framework.Backoffice
{
    public class SystemService : ISystemService<BorgSettings>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISerializer _serializer;
        public BorgSettings Settings { get; }

        public SystemService(ILoggerFactory loggerFactory, BorgSettings settings, ISerializer serializer)
        {
            Settings = settings;
            _loggerFactory = loggerFactory;
            _serializer = serializer;
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

        public Task<object> DeserializeAsync(byte[] data, Type objectType)
        {
            return _serializer.DeserializeAsync(data, objectType);
        }

        public Task<byte[]> SerializeAsync(object value)
        {
            return _serializer.SerializeAsync(value);
        }
    }
}