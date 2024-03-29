using Borg.Infra.Relational;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Borg.Infra.EF6
{
    public delegate Task PreProcessQueryAsyncHandler<T>(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, Expression<Func<T, dynamic>>[] paths, out IPagedResult<T> result) where T : class;

    public delegate Task PostProcessQueryAsyncHandler<T>(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, Expression<Func<T, dynamic>>[] paths, IPagedResult<T> result) where T : class;

    public abstract class BaseQueryRespository<T, TDbContext> : IQueryRepository<T> where T : class where TDbContext : DbContext
    {
        protected virtual PreProcessQueryAsyncHandler<T> PreProcess { get; set; } = null;
        protected virtual PostProcessQueryAsyncHandler<T> PostProcess { get; set; } = null;

        public abstract TDbContext DbContext { get; }

        public async Task<IPagedResult<T>> FindAsync(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, params Expression<Func<T, dynamic>>[] paths)
        {
            IPagedResult<T> result;
            await PreProcess?.Invoke(predicate, page, size, orderBy, paths, out result);
            result = await DbContext.FetchAsync(predicate, page, size, orderBy, paths, true);
            await PostProcess?.Invoke(predicate, page, size, orderBy, paths, result);
            return result;
        }
    }
}