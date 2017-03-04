using System;
using System.Collections.Generic;
using System.Linq;

namespace Borg.Infra.Relational
{
    public class UoWScopeStack<TContext> : IDisposable where TContext : IDisposable
    {
        private bool _isRolledBack;
        private readonly IUniqueKeyProvider<Guid> _keys = new GuidKeyProvider();

        public UoWScopeStack(TContext context)
        {
            Context = context;
            Stack = new Stack<IUnitOfWork>();

            Id = _keys.Pop().ToString();
        }

        public TContext Context { get; protected set; }
        public Stack<IUnitOfWork> Stack { get; protected set; }

        public string Id { get; private set; }

        //public ITransactionWrapper Transaction { get; private set; }

        public void Dispose()
        {
            //ScopedUnitOfWorkConfiguration.LoggingAction(this + "disposing, will dispose context and transaction");

            Context.Dispose();
            //Transaction?.Dispose();
        }

        public bool IsRolledBack()
        {
            return _isRolledBack;
        }

        public void RollBack()
        {
            //ScopedUnitOfWorkConfiguration.LoggingAction(this + "rollback requested");

            //Transaction.Rollback();
            _isRolledBack = true;
        }

        public void CleanTransaction()
        {
            _isRolledBack = false;

            //  ScopedUnitOfWorkConfiguration.LoggingAction(this + "clean transaction requested, transaction exists: " + (Transaction != null));

            //if (Transaction != null)
            //{
            //    Transaction.Dispose();
            //    Transaction = null;
            //}
        }

        public bool HasTransaction()
        {
            return false; //Transaction != null;
        }

        public void SetTransaction(/*ITransactionWrapper transaction*/)
        {
            //Transaction = transaction;
        }

        public bool AnyTransactionalUnitsOfWork()
        {
            return false;
        }

        public bool AnyTransactionalUnitsOfWorkBesides(IUnitOfWork unitOfWork)
        {
            return false;
        }

        public override string ToString()
        {
            string stackItems = Stack.Aggregate("", (current, item) => current + item.ToString());

            return $"[UowScopeStack {Id}] <<<" + stackItems + ">>> |  ";
        }
    }
}