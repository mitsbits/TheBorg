using Mehdime.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Borg.Infra.Relational.EF6
{
    public class ConnectionStringsDictionaryDbContextFactory : IDbContextFactory
    {
        private readonly IDictionary<Type, string> _connectionStrings;

        public ConnectionStringsDictionaryDbContextFactory(IDictionary<Type, string> connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            var type = typeof(TDbContext);
            if (!_connectionStrings.ContainsKey(typeof(TDbContext)))
                throw new Exception($"no connection string is registered for {type}");
            var connString = _connectionStrings[type];
            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), connString);
        }
    }
}