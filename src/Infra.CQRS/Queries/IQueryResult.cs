namespace Borg.Infra.CQRS
{
    public interface IQueryResult<T> : IPagedResult<T>, IQueryResult
    {
    }

    public interface IQueryResult : IResponse
    {
        bool Success { get; }
        string Description { get; }
    }
}