using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public interface IQueryBus
    {
        Task<IQueryResult<V>> Fetch<T, V>(T request) where T : IQueryRequest where V : IResponse;
    }
}