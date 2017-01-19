using Borg.Infra.Relational;
using System.Collections.Generic;

namespace Borg.Infra.CQRS
{
    public abstract class QueryResult<TEntity> : PagedResult<TEntity>, IQueryResult<TEntity>
    {
        protected QueryResult(IEnumerable<TEntity> data, int pageNumber, int pageSize, int totalRecords)
            : base(data, pageNumber, pageSize, totalRecords)
        {
            Success = true;
            Description = string.Empty;
        }

        protected QueryResult(bool success, string description) : this(new TEntity[0], 1, 1, 0)
        {
            Success = success;
            Description = description;
        }

        public string Description { get; }

        public bool Success { get; }
    }
}