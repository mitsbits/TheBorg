using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Borg.Infra.Relational
{
    public interface IWriteAsyncRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity);

        Task<T> UpdateAsync(T entity);

        Task DeleteAsync(Expression<Func<T, bool>> predicate);

        Task DeleteAsync(T entity);
    }
}