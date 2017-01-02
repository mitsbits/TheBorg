using System.Collections.Generic;

namespace Borg.Infra.CQRS
{
    public class CollectionQueryResult<T> : QueryResult<T>
    {
        public CollectionQueryResult(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords) : base(data, pageNumber, pageSize, totalRecords)
        {
        }
    }
}