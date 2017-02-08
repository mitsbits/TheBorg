using System;
using Microsoft.Extensions.Logging;

namespace Borg.Framework.Services
{
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory();
        public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
        public static ILogger CreateLogger(Type type) => LoggerFactory.CreateLogger(type);
    }
}