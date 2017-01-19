using Borg.Infra.Relational;

namespace Borg.Infra.CQRS
{
    public interface IQueryResult<out T> : IPagedResult<T>, IQueryResult
    {
    }

    public interface IQueryResult : IResponse
    {
        bool Success { get; }
        string Description { get; }
    }
}