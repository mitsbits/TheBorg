using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Borg.Infra.EFCore
{
    public class ConnectionStringsDictionaryDbContextFactory : IDbContextFactory
    {
        private readonly IDictionary<Type, string> _connectionStrings;
        private ILogger Logger { get; }

        public ConnectionStringsDictionaryDbContextFactory(IDictionary<Type, string> connectionStrings, ILoggerFactory loggerFactory)
        {
            _connectionStrings = connectionStrings;
            Logger = loggerFactory.CreateLogger(GetType());
        }

        public TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            var type = typeof(TDbContext);
            if (!_connectionStrings.ContainsKey(typeof(TDbContext)))
                throw new Exception($"no connection string is registered for {type}");
            var connString = _connectionStrings[type];
            Logger.LogDebug("Creating {DbContext} for {ConnectionString}", typeof(TDbContext), connString);
            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), connString);
        }
    }
}