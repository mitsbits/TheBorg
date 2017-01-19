namespace Borg.Infra.CQRS
{
    public class ScalarQueryResult<TEntity> : QueryResult<TEntity>
    {
        public ScalarQueryResult(TEntity data) : base(new[] { data }, 1, 1, 1)
        {
        }
    }
}