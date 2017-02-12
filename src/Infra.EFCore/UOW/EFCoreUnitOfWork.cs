using System;
using Borg.Infra.Relational;
using Microsoft.EntityFrameworkCore;

namespace Borg.Infra.EFCore
{
    public class EFCoreUnitOfWork<TContext> : UnitOfWork<TContext> where TContext : DbContext
    {
        public EFCoreUnitOfWork(TContext context, IScopeManager scopeManager, IServiceProvider serviceLocator/*, ScopeType scopeType*/)
            : base(context, scopeManager, serviceLocator/*, scopeType*/)
        {
        }

        protected override void SaveContextChanges()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                throw new ConcurrentModificationException(
                    "The record you attempted to edit was modified by another " +
                    "user after you loaded it. The edit operation was cancelled and the " +
                    "currect values in the database are displayed. Please try again.", exception);
            }
        }
    }
}