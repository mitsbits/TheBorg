using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Borg.Infra.EFCore
{
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
}