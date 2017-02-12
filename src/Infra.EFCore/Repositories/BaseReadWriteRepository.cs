using Borg.Infra.Relational;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Borg.Infra.EFCore
{
    public abstract class BaseReadWriteRepository<T, TDbContext> : BaseReadRepository<T, TDbContext>,
        IWriteRepository<T>, IWriteAsyncRepository<T>
        where TDbContext :
        DbContext
        where T : class
    {
        //protected virtual string GetEntitySetName(Type type)
        //{
        //    var container = ((IObjectContextAdapter)DbContext)
        //        .ObjectContext.MetadataWorkspace.GetEntityContainer(((IObjectContextAdapter)DbContext).ObjectContext.DefaultContainerName, DataSpace.CSpace);
        //    return (from meta in container.BaseEntitySets
        //            where meta.ElementType.Name == type.Name
        //            select meta.Name).First();
        //}

        public virtual void Create(T entity)
        {
            //InternallPreProcess();
            DbContext.Set<T>().Add(entity);
            //InternallPostProcessUpdate();
        }

        public virtual void Update(T entity)
        {
            //InternallPreProcess();
            DbContext.Set<T>().Update(entity);
        }

        public virtual void Delete(T entity)
        {
            DbContext.Set<T>().Attach(entity);
            DbContext.Entry(entity).State = EntityState.Deleted;
            DbContext.Set<T>().Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            //InternallPreProcess();
            var items = DbContext.Set<T>().Where(predicate);
            foreach (var item in items)
            {
                Delete(item);
            }
            //InternallPostProcessUpdate();
        }

        #region Async

        public virtual async Task<T> CreateAsync(T entity)
        {
            //await InternallPreProcessAsync();
            DbContext.Set<T>().Add(entity);
            //await InternallPostProcessUpdateAsync();
            return entity;
        }

        public virtual Task<T> UpdateAsync(T entity)
        {
            var result = DbContext.Set<T>().Update(entity).Entity;
            return Task.FromResult(result);
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            // await InternallPreProcessAsync();
            var items = await DbContext.Set<T>().Where(predicate).ToListAsync();
            foreach (var item in items)
            {
                DbContext.Set<T>().Remove(item);
            }
            // await InternallPostProcessUpdateAsync();
        }

        public Task DeleteAsync(T entity)
        {
            //    await InternallPreProcessAsync();
            DbContext.Set<T>().Attach(entity);
            DbContext.Entry(entity).State = EntityState.Deleted;
            DbContext.Set<T>().Remove(entity);
            //await InternallPostProcessUpdateAsync();
            return Task.CompletedTask;
        }

        #endregion Async
    }

    public abstract class UowRepository<T, TDbContext> : BaseReadWriteRepository<T, TDbContext>, IUowRepository where TDbContext :
        DbContext
        where T : class
    {
        protected IContextAwareUnitOfWork UnitOfWork { get; private set; }

        public void SetUnitOfWork(IUnitOfWork unitOfWork)
        {
            UnitOfWork = (IContextAwareUnitOfWork)unitOfWork;
        }

        public override TDbContext DbContext
        {
            get
            {
                var context = UnitOfWork.GetContext() as TDbContext;
                if (context == null)
                    throw CreateIncorrectContextTypeException();
                return context;
            }
        }

        protected Exception CreateIncorrectContextTypeException()
        {
            return new IncorrectUnitOfWorkUsageException(
                        "Unit of work registered context: " + UnitOfWork.GetContext() +
                        ", type: " + UnitOfWork.GetContext()?.GetType() +
                        " could not be cast to expected EF DbContext base class. " +
                        "Make sure the context you registered the unit of work factory with " +
                        "is correctly registed with IoC container (as self) and that it inherits " +
                        "from EF DbContext.");
        }
    }
}