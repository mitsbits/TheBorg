using System;
using Borg.Infra.Relational;
using Microsoft.EntityFrameworkCore;

namespace Borg.Infra.EFCore
{
    internal class UnitOfWorkScopeManager<TContext> : UnitOfWorkScopeManagerBase<TContext> where TContext : DbContext
    {
        private readonly IServiceProvider _serviceLocator;

        public UnitOfWorkScopeManager(IServiceProvider serviceLocator) : base(serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        protected override IUnitOfWork CreateUnitOfWork(/*ScopeType scopeType*/)
        {
            return new EFCoreUnitOfWork<TContext>(ScopeStack.Context, this, _serviceLocator/*, scopeType*/);
        }

        //protected override ITransactionWrapper CreateAndStartTransaction()
        //{
        //    return new TransactionWrapper(ScopeStack.Context.Database.BeginTransaction());
        //}

        //class TransactionWrapper : ITransactionWrapper
        //{
        //    private readonly IRelationalTransaction _transaction;

        //    public TransactionWrapper(IRelationalTransaction transaction)
        //    {
        //        _transaction = transaction;
        //    }

        //    public void Dispose()
        //    {
        //        _transaction.Dispose();
        //    }

        //    public void Commit()
        //    {
        //        _transaction.Commit();
        //    }

        //    public void Rollback()
        //    {
        //        _transaction.Rollback();
        //    }
        //}
    }
}