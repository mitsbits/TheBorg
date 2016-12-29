using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Borg.Infra.Relational
{
    public interface IReadAsyncRepository<T> where T : class
    {
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, dynamic>>[] paths);

        Task<IPagedResult<T>> FindAsync(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, params Expression<Func<T, dynamic>>[] paths);

        Task<IPagedResult<T>> FindAsync(Expression<Func<T, bool>> predicate, IEnumerable<OrderByInfo<T>> orderBy, params Expression<Func<T, dynamic>>[] paths);
    }
}