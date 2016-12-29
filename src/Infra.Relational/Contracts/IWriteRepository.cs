using System;
using System.Linq.Expressions;

namespace Borg.Infra.Relational
{
    public interface IWriteRepository<T> where T : class
    {
        void Create(T entity);

        void Update(T entity);

        void Delete(T entity);

        void Delete(Expression<Func<T, bool>> predicate);
    }
}