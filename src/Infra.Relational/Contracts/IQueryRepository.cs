using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Borg.Infra.Relational
{
    public interface IQueryRepository<T> where T : class
    {
        Task<IPagedResult<T>> FindAsync(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy);

        Task<IPagedResult<T>> FindAsync(Expression<Func<T, bool>> predicate, IEnumerable<OrderByInfo<T>> orderBy);
    }
}