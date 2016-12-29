using System.Collections.Generic;

namespace Borg.Infra
{
    public interface IPagedResult<T> : IList<T>
    {
        IList<T> Records { get; }

        int Page { get; }

        bool HasNextPage { get; }

        bool HasPreviousPage { get; }

        int PageSize { get; }

        int TotalRecords { get; }

        int TotalPages { get; }
    }
}