namespace Borg.Infra.CQRS
{
    public class ScalarQueryResult<T> : QueryResult<T>
    {
        public ScalarQueryResult(T data) : base(new[] { data }, 1, 1, 1)
        {
        }
    }
}