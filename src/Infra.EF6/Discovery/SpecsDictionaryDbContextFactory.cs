using Mehdime.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Borg.Infra.EF6
{
    public class SpecsDictionaryDbContextFactory : IDbContextFactory
    {
        private readonly IDictionary<Type, DiscoveryDbContextSpec> _specs;

        public SpecsDictionaryDbContextFactory(IDictionary<Type, DiscoveryDbContextSpec> specs)
        {
            _specs = specs;
        }

        public TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            var type = typeof(TDbContext);
            if (!_specs.ContainsKey(typeof(TDbContext)))
                throw new Exception($"no DiscoveryDbContextSpec is registered for {type}");
            var spec = _specs[type];
            if (typeof(TDbContext).GetInterfaces().Contains(typeof(IDiscoveryDbContext)))
            {
                return (TDbContext)Activator.CreateInstance(typeof(TDbContext), spec);
            }
            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), spec.ConnectionStringOrName);
        }
    }
}