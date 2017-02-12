using System;
using Borg.Infra.Relational;
using Microsoft.EntityFrameworkCore;

namespace Borg.Infra.EFCore
{
    public class UnitOfWorkFactory<TContext> : UnitOfWorkFactoryBase<TContext> where TContext : DbContext
    {
        public UnitOfWorkFactory(IServiceProvider serviceLocator) :
            base(new UnitOfWorkScopeManager<TContext>(serviceLocator))
        {
        }
    }
}