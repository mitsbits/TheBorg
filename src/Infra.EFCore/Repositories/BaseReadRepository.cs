using Borg.Infra.Relational;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Borg.Infra.EFCore
{
    public delegate Task PreProcessReadAsyncHandler<T>(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, Expression<Func<T, dynamic>>[] paths, out IPagedResult<T> result) where T : class;

    public delegate Task PostProcessReadAsyncHandler<T>(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, Expression<Func<T, dynamic>>[] paths, IPagedResult<T> result) where T : class;

    public delegate void PreProcessReadHandler<T>(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, Expression<Func<T, dynamic>>[] paths, out IPagedResult<T> result) where T : class;

    public delegate void PostProcessReadHandler<T>(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, Expression<Func<T, dynamic>>[] paths, IPagedResult<T> result) where T : class;

    public abstract class BaseReadRepository<T, TDbContext> : IReadRepository<T>, IReadAsyncRepository<T> where T : class where TDbContext : DbContext
    {
        public abstract TDbContext DbContext { get; }

        protected virtual PreProcessReadHandler<T> PreProcessRead { get; set; } = null;
        protected virtual PostProcessReadHandler<T> PostProcessRead { get; set; } = null;

        public virtual IPagedResult<T> Find(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, params Expression<Func<T, dynamic>>[] paths)
        {
            IPagedResult<T> result;
            PreProcessRead?.Invoke(predicate, page, size, orderBy, paths, out result);
            result = DbContext.Fetch(predicate, page, size, orderBy, paths);
            PostProcessRead?.Invoke(predicate, page, size, orderBy, paths, result);
            return result;
        }

        #region Async

        protected virtual PreProcessReadAsyncHandler<T> PreProcessReadAsync { get; set; } = null;
        protected virtual PostProcessReadAsyncHandler<T> PostProcessReadAsync { get; set; } = null;

        public async Task<IPagedResult<T>> FindAsync(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, params Expression<Func<T, dynamic>>[] paths)
        {
            IPagedResult<T> result;
            await PreProcessReadAsync?.Invoke(predicate, page, size, orderBy, paths, out result);
            result = await DbContext.FetchAsync(predicate, page, size, orderBy, paths);
            await PostProcessReadAsync?.Invoke(predicate, page, size, orderBy, paths, result);
            return result;
        }

        #endregion Async
    }
}