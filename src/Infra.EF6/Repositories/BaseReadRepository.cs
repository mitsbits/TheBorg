using Borg.Infra.Relational;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Borg.Infra.EF6
{
    public abstract class BaseReadRepository<T, TDbContext> : BaseRepository, IReadRepository<T>, IReadAsyncRepository<T> where T : class where TDbContext : DbContext
    {
        public abstract TDbContext DbContext { get; }

        public virtual T Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, dynamic>>[] paths)
        {
            InternallPreProcess();
            var query = DbContext.Set<T>().Where(predicate);
            if (paths != null && paths.Any())
            {
                query = paths.Aggregate(query, (current, path) => current.Include(path));
            }
            var result = query.FirstOrDefault();
            InternallPostProcessQuery();
            return result;
        }

        public virtual IPagedResult<T> Find(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, params Expression<Func<T, dynamic>>[] paths)
        {
            InternallPreProcess();
            IPagedResult<T> result;
            var query = DbContext.Set<T>().Where(predicate);
            var totalRecords = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / size);
            if (page > totalPages) { page = totalPages; }
            if (totalRecords == 0)
            {
                result = new PagedResult<T>(new List<T>(), page, size, 0);
            }
            else
            {
                if (paths != null && paths.Any())
                {
                    query = paths.Aggregate(query, (current, path) => current.Include(path));
                }

                var orderByInfos = orderBy as OrderByInfo<T>[] ?? orderBy.ToArray();
                IOrderedQueryable<T> orderedQueryable = query.Apply(orderByInfos);

                if (orderedQueryable == null)
                {
                    result = new PagedResult<T>(new List<T>(), page, size, 0);
                }
                else
                {
                    var data = orderedQueryable.Skip((page - 1) * size).Take(size).ToList();
                    result = new PagedResult<T>(data, page, size, totalRecords);
                }
            }
            InternallPostProcessQuery();
            return result;
        }

        public virtual IPagedResult<T> Find(Expression<Func<T, bool>> predicate, IEnumerable<OrderByInfo<T>> orderBy, params Expression<Func<T, dynamic>>[] paths)
        {
            InternallPreProcess();
            IPagedResult<T> result;
            var query = DbContext.Set<T>().Where(predicate);

            if (paths != null && paths.Any())
            {
                query = paths.Aggregate(query, (current, path) => current.Include(path));
            }

            var orderByInfos = orderBy as OrderByInfo<T>[] ?? orderBy.ToArray();
            IOrderedQueryable<T> orderedQueryable = query.Apply(orderByInfos);

            if (orderedQueryable == null) { result = new PagedResult<T>(new List<T>(), 1, 0, 0); }
            else
            {
                var data = orderedQueryable.ToList();
                var count = data.Count();
                result = new PagedResult<T>(data, 1, count, count);
            }
            InternallPostProcessQuery();
            return result;
        }

        public virtual bool Contains(Expression<Func<T, bool>> predicate)
        {
            InternallPreProcess();
            var result = DbContext.Set<T>().Any(predicate);
            InternallPostProcessQuery();
            return result;
        }

        #region Async

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, dynamic>>[] paths)
        {
            await InternallPreProcessAsync();
            var query = DbContext.Set<T>().Where(predicate);
            if (paths != null && paths.Any())
            {
                query = paths.Aggregate(query, (current, path) => current.Include(path));
            }
            var result = await query.FirstOrDefaultAsync();
            await InternallPostProcessQueryAsync();
            return result;
        }

        public async Task<IPagedResult<T>> FindAsync(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, params Expression<Func<T, dynamic>>[] paths)
        {
            await InternallPreProcessAsync();
            IPagedResult<T> result;
            var query = DbContext.Set<T>().Where(predicate);
            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / page);
            if (page > totalPages) { page = totalPages; }
            if (totalRecords == 0)
            {
                result = new PagedResult<T>(new List<T>(), page, size, 0);
            }
            else
            {
                if (paths != null && paths.Any())
                {
                    query = paths.Aggregate(query, (current, path) => current.Include(path));
                }
                var orderByInfos = orderBy as OrderByInfo<T>[] ?? orderBy.ToArray();
                IOrderedQueryable<T> orderedQueryable = query.Apply(orderByInfos);

                if (orderedQueryable == null)
                {
                    result = new PagedResult<T>(new List<T>(), page, size, 0);
                }
                else
                {
                    var data = await orderedQueryable.Skip((page - 1) * size).Take(size).ToListAsync();
                    result = new PagedResult<T>(data, page, size, totalRecords);
                }
            }
            await InternallPostProcessQueryAsync();
            return result;
        }

        public async Task<IPagedResult<T>> FindAsync(Expression<Func<T, bool>> predicate, IEnumerable<OrderByInfo<T>> orderBy, params Expression<Func<T, dynamic>>[] paths)
        {
            await InternallPreProcessAsync();
            IPagedResult<T> result;
            var query = DbContext.Set<T>().Where(predicate);

            if (paths != null && paths.Any())
            {
                query = paths.Aggregate(query, (current, path) => current.Include(path));
            }
            var orderByInfos = orderBy as OrderByInfo<T>[] ?? orderBy.ToArray();
            IOrderedQueryable<T> orderedQueryable = query.Apply(orderByInfos);

            if (orderedQueryable == null)
            {
                result = new PagedResult<T>(new List<T>(), 1, 0, 0);
            }
            else
            {
                var data = await orderedQueryable.ToListAsync();
                var count = data.Count();
                result = new PagedResult<T>(data, 1, count, count);
            }
            await InternallPostProcessQueryAsync();
            return result;
        }

        #endregion Async
    }
}