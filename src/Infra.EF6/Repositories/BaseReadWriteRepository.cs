using Borg.Infra.Relational;
using System;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Borg.Infra.EF6
{
    public abstract class BaseReadWriteRepository<T, TDbContext> : BaseReadRepository<T, TDbContext>,
        IWriteRepository<T>, IWriteAsyncRepository<T>
        where TDbContext :
        DbContext
        where T : class
    {
        protected virtual string GetEntitySetName(Type type)
        {
            var container = ((IObjectContextAdapter)DbContext)
                .ObjectContext.MetadataWorkspace.GetEntityContainer(((IObjectContextAdapter)DbContext).ObjectContext.DefaultContainerName, DataSpace.CSpace);
            return (from meta in container.BaseEntitySets
                    where meta.ElementType.Name == type.Name
                    select meta.Name).First();
        }

        public virtual void Create(T entity)
        {
            InternallPreProcess();
            DbContext.Set<T>().Add(entity);
            InternallPostProcessUpdate();
        }

        public virtual void Update(T entity)
        {
            InternallPreProcess();
            var entitySet = GetEntitySetName(typeof(T));
            var key = ((IObjectContextAdapter)DbContext).ObjectContext.CreateEntityKey(entitySet, entity);
            object originalItem;
            if (((IObjectContextAdapter)DbContext).ObjectContext.TryGetObjectByKey(key, out originalItem))
            {
                DbContext.Entry(originalItem).CurrentValues.SetValues(entity);
            }
            InternallPostProcessUpdate();
        }

        public virtual void Delete(T entity)
        {
            InternallPreProcess();
            var entitySet = this.GetEntitySetName(typeof(T));
            EntityKey key = ((IObjectContextAdapter)DbContext).ObjectContext.CreateEntityKey(entitySet, entity);

            object originalItem;
            if (((IObjectContextAdapter)DbContext).ObjectContext.TryGetObjectByKey(key, out originalItem))
            {
                DbContext.Set<T>().Remove(entity);
            }
            InternallPostProcessUpdate();
        }

        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            InternallPreProcess();
            var items = DbContext.Set<T>().Where(predicate);
            foreach (var item in items)
            {
                Delete(item);
            }
            InternallPostProcessUpdate();
        }

        #region Async

        public virtual async Task<T> CreateAsync(T entity)
        {
            await InternallPreProcessAsync();
            DbContext.Set<T>().Add(entity);
            await InternallPostProcessUpdateAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            await InternallPreProcessAsync();
            var entitySet = GetEntitySetName(typeof(T));
            var key = ((IObjectContextAdapter)DbContext).ObjectContext.CreateEntityKey(entitySet, entity);
            object originalItem;
            if (((IObjectContextAdapter)DbContext).ObjectContext.TryGetObjectByKey(key, out originalItem))
            {
                DbContext.Entry(originalItem).CurrentValues.SetValues(entity);
            }
            await InternallPostProcessUpdateAsync();
            return entity;
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            await InternallPreProcessAsync();
            var items = await DbContext.Set<T>().Where(predicate).ToListAsync();
            foreach (var item in items)
            {
                DbContext.Set<T>().Remove(item);
            }
            await InternallPostProcessUpdateAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            await InternallPreProcessAsync();
            DbContext.Set<T>().Remove(entity);
            await InternallPostProcessUpdateAsync();
        }

        #endregion Async
    }
}