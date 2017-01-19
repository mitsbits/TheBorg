using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public interface IHandlesQueryRequest<in T> where T : IQueryRequest
    {
        Task<IQueryResult> Execute(T message);
    }

    public interface IHandlesQueryRequest<in T, TEntity> where T : IQueryRequest<TEntity>
    {
        Task<IQueryResult<TEntity>> Execute(T message);
    }
}