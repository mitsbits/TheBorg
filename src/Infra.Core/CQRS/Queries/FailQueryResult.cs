namespace Borg.Infra.CQRS
{
    public class FailQueryResult<T> : QueryResult<T>, IQueryRequest<T>
    {
        public FailQueryResult(string description = "") : base(false, description)
        {
        }
    }
}