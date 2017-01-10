using Borg.Infra.Relational;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Borg.Infra.Relational
{
    public interface IReadAsyncRepository<T> where T : class
    {
        Task<IPagedResult<T>> FindAsync(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, params Expression<Func<T, dynamic>>[] paths);
    }
}

namespace Borg
{
    public static class IReadAsyncRepositoryExtensions
    {
        public static async Task<IPagedResult<T>> FindAsync<T>(this IReadAsyncRepository<T> repo, Expression<Func<T, bool>> predicate, IEnumerable<OrderByInfo<T>> orderBy, params Expression<Func<T, dynamic>>[] paths) where T : class
        {
            return await repo.FindAsync(predicate, -1, -1, null, paths);
        }

        public static async Task<T> GetAsync<T>(this IReadAsyncRepository<T> repo, Expression<Func<T, bool>> predicate, params Expression<Func<T, dynamic>>[] paths) where T : class
        {
            var data = await repo.FindAsync(predicate, 1, 1, null, paths);
            return (data.TotalRecords > 0) ? data.Records[0] : null;
        }
    }
}