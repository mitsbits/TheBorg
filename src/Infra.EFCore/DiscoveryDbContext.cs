using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Borg.Infra.EFCore
{
    public class DiscoveryDbContext : DbContext
    {
        public DiscoveryDbContext() : base() { }
    }

    public class DiscoveryDbContextOptions : DbContextOptions
    {
        public DiscoveryDbContextOptions(IReadOnlyDictionary<Type, IDbContextOptionsExtension> extensions) : base(extensions)
        {
        }

        public override DbContextOptions WithExtension<TExtension>(TExtension extension)
        {
            throw new NotImplementedException();
        }

        public override Type ContextType { get; }
    }


}