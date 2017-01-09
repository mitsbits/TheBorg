using Borg.Infra.Relational;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Borg.Infra.Relational
{
    public interface IReadRepository<T> where T : class
    {
        IPagedResult<T> Find(Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, params Expression<Func<T, dynamic>>[] paths);
    }
}

namespace Borg
{
    public static class IReadRepositoryExtensions
    {
        public static IPagedResult<T> Find<T>(this IReadRepository<T> repo, Expression<Func<T, bool>> predicate,
            IEnumerable<OrderByInfo<T>> orderBy, params Expression<Func<T, dynamic>>[] paths) where T : class
        {
            return repo.Find(predicate, -1, -1, null, paths);
        }

        public static T Get<T>(this IReadRepository<T> repo, Expression<Func<T, bool>> predicate, params Expression<Func<T, dynamic>>[] paths) where T : class
        {
            var data = repo.Find(predicate, 1, 1, null, paths);
            return (data.TotalRecords > 0) ? data.Records[0] : null;
        }

        public static bool Contains<T>(this IReadRepository<T> repo, Expression<Func<T, bool>> predicate) where T : class
        {
            var data = repo.Find(predicate, 1, 1, null);
            return (data.TotalRecords > 0);
        }
    }
}