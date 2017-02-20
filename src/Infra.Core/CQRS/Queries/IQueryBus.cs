using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public interface IQueryBus
    {
        Task<IQueryResult> Fetch<T>(T request) where T : IQueryRequest;
    }
}