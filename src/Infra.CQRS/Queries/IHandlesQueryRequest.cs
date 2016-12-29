using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public interface IHandlesQueryRequest<in T> where T : IQueryRequest
    {
        Task<IQueryResult<V>> Execute<V>(T message) where V : IResponse;
    }
}