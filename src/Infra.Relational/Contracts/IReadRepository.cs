using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Borg.Infra.Relational
{
    public interface IReadRepository<T> where T : class
    {
        T Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, dynamic>>[] paths);

        IPagedResult<T> Find(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, params Expression<Func<T, dynamic>>[] paths);

        IPagedResult<T> Find(Expression<Func<T, bool>> predicate, IEnumerable<OrderByInfo<T>> orderBy, params Expression<Func<T, dynamic>>[] paths);

        bool Contains(Expression<Func<T, bool>> predicate);
    }
}