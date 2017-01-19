namespace Borg.Infra.CQRS
{
    public interface IQueryRequest
    {
    }

    public interface IQueryRequest<in TEntity> : IQueryRequest
    {
    }
}