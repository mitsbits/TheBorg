using System.Collections.Generic;

namespace Borg.Infra.Relational
{
    public interface IPagedResult<T> : IPagedResult, IList<T>
    {
        IList<T> Records { get; }

    }

    public interface IPagedResult
    {
        int Page { get; }
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
        int PageSize { get; }
        int TotalRecords { get; }
        int TotalPages { get; }
    }
}