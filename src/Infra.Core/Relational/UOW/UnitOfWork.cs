using Microsoft.Extensions.Logging;
using System;

namespace Borg.Infra.Relational
{
    public abstract class UnitOfWork<TContext> : IContextAwareUnitOfWork where TContext : class, IDisposable
    {
        protected readonly TContext Context;
        private readonly IServiceProvider _serviceLocator;
        private readonly IScopeManager _scopeManager;
        private ILogger Logger { get; }
        private readonly IUniqueKeyProvider<Guid> _keys = new GuidKeyProvider();

        protected UnitOfWork(TContext context, IScopeManager scopeManager, IServiceProvider serviceLocator)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (scopeManager == null) throw new ArgumentNullException(nameof(scopeManager));
            if (serviceLocator == null) throw new ArgumentNullException(nameof(serviceLocator));

            Context = context;
            _scopeManager = scopeManager;
            _serviceLocator = serviceLocator;

            Id = _keys.Pop().ToString();

            var loggerFactory = _serviceLocator.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            if (loggerFactory != null)
            {
                Logger = loggerFactory.CreateLogger(GetType());
            }
            //ScopeType = scopeType;
        }

        public object GetContext()
        {
            return Context;
        }

        public TRepository GetRepository<TRepository>() where TRepository : class, IUowRepository
        {
            var repository = _serviceLocator.GetService(typeof(TRepository)) as IUowRepository;
            repository.SetUnitOfWork(this);
            return repository as TRepository;
        }

        public string Id { get; private set; }

        //public ScopeType ScopeType { get; private set; }

        public bool IsFinished { get; private set; }

        public void Commit()
        {
            if (IsFinished)
                throw new InvalidOperationException("Unit of work could not be commited either because it was " +
                                                    "already commited or it was disposed.");

            _scopeManager.Complete(this);

            try
            {
                Logger.LogDebug("Persisting context changes... {@UoW}", this);
                SaveContextChanges();
            }
            finally
            {
                IsFinished = true;
            }
        }

        public override string ToString()
        {
            return $"[UnitOfWork {Id}] ";
        }

        protected abstract void SaveContextChanges();

        public void Dispose()
        {
            Logger.LogDebug("Disposing... {@UoW}", this);
            _scopeManager.Remove(this);

            IsFinished = true;
        }
    }
}