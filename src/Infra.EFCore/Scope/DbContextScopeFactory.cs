using System;
using Borg.Infra.Relational;
using Microsoft.Extensions.DependencyInjection;

namespace Borg.Infra.EFCore
{
    public class DbContextScopeFactory : IDbContextScopeFactory
    {
        private readonly IDbContextFactory _dbContextFactory;

        public DbContextScopeFactory(IDbContextFactory dbContextFactory = null)
        {
            _dbContextFactory = dbContextFactory;
        }

        public IDbContextScope Create()
        {
            return new DbContextScope(
                    /* joiningOption: DbContextScopeOption.ForceCreateNew,*/
                    readOnly: false,
                    /* isolationLevel: isolationLevel,*/
                    dbContextFactory: _dbContextFactory);
        }

        //public IDbContextScope Create(DbContextScopeOption joiningOption = DbContextScopeOption.JoinExisting)
        //{
        //    return new DbContextScope(
        //        joiningOption: joiningOption,
        //        readOnly: false,
        //        isolationLevel: null,
        //        dbContextFactory: _dbContextFactory);
        //}

        public IDbContextReadOnlyScope CreateReadOnly(/*DbContextScopeOption joiningOption = DbContextScopeOption.JoinExisting*/)
        {
            return new DbContextReadOnlyScope(
                /*joiningOption: joiningOption,
                isolationLevel: null,*/
                dbContextFactory: _dbContextFactory);
        }

        //public IDbContextScope CreateWithTransaction(/*IsolationLevel isolationLevel*/)
        //{
        //    return new DbContextScope(
        //        /* joiningOption: DbContextScopeOption.ForceCreateNew,*/
        //        readOnly: false,
        //        /* isolationLevel: isolationLevel,*/
        //        dbContextFactory: _dbContextFactory);
        //}

        //public IDbContextReadOnlyScope CreateReadOnlyWithTransaction(IsolationLevel isolationLevel)
        //{
        //    return new DbContextReadOnlyScope(
        //        joiningOption: DbContextScopeOption.ForceCreateNew,
        //        isolationLevel: isolationLevel,
        //        dbContextFactory: _dbContextFactory);
        //}

        public IDisposable SuppressAmbientContext()
        {
            return new AmbientContextSuppressor();
        }

        public virtual TRepository CreateRepo<TRepository>() where TRepository : IRepository
        {
            throw new NotImplementedException();
        }
    }



    public class ServiceLocatorDbContextScopeFactory : DbContextScopeFactory
    {
        private readonly IServiceProvider _serviceLocator;

        public ServiceLocatorDbContextScopeFactory(IServiceProvider serviceLocator, IDbContextFactory dbContextFactory = null) : base(dbContextFactory)
        {
            _serviceLocator = serviceLocator;
        }

        public override TRepository CreateRepo<TRepository>()
        {
        
                var obj = _serviceLocator.GetService(typeof(TRepository));
                return (TRepository)obj;
            
        }
    }
}