using System.Collections.Generic;
using Borg.Infra.Relational;

namespace Borg.Infra.CQRS
{
    public abstract class QueryResult<T> : PagedResult<T>, IQueryResult<T>
    {
        protected QueryResult(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
            : base(data, pageNumber, pageSize, totalRecords)
        {
            Success = true;
            Description = string.Empty;
        }

        protected QueryResult(bool success, string description) : this(new T[0], 1, 1, 0)
        {
            Success = success;
            Description = description;
        }

        public string Description { get; }

        public bool Success { get; }
    }
}