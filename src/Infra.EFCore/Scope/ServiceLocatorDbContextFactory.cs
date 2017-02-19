using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Borg.Infra.EFCore
{
    [Obsolete("Do not use, this is eveil", true)]
    public class ServiceLocatorDbContextFactory : IDbContextFactory
    {
        public ServiceLocatorDbContextFactory(IServiceProvider serviceLocator)
        {
            ServiceLocator = serviceLocator;
        }

        private  IServiceProvider ServiceLocator { get; }

        public TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            return ServiceLocator.GetService<TDbContext>();
        }
    }


    public class FactoryDbContextFactory : IDbContextFactory
    {
    
        public FactoryDbContextFactory(IServiceProvider serviceLocator)
        {
            ServiceLocator = serviceLocator;
            Options = ServiceLocator.GetService<DbContextFactoryOptions>();
        }

        private DbContextFactoryOptions Options { get; }

        private IServiceProvider ServiceLocator { get; }

        public TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            var f = ServiceLocator.GetService(typeof(IDbContextFactory<TDbContext>)) as IDbContextFactory<TDbContext>;
            return f?.Create(Options);
        }
    }
}