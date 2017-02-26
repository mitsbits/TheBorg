using Borg.Infra.CQRS;
using Borg.Infra.Relational;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Borg
{
    internal static class ReadFromDbExtensions
    {
        public static async Task<IPagedResult<T>> FetchAsync<T, TDbContext>(this TDbContext db, Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, Expression<Func<T, dynamic>>[] paths, bool readOnly = false) where T : class where TDbContext : DbContext
        {
            var fetchAll = (page == -1 && size == -1);
            IPagedResult<T> result;

            var query = (readOnly) ? db.Set<T>().AsNoTracking().Where(predicate) : db.Set<T>().Where(predicate);
            var totalRecords = (fetchAll) ? 0 : await query.CountAsync();
            var totalPages = (fetchAll) ? 1 : (int)Math.Ceiling((double)totalRecords / size);
            if (page > totalPages)
            {
                page = totalPages;
            }
            if (!fetchAll && totalRecords == 0)
            {
                result = new PagedResult<T>(new List<T>(), page, size, 0);
            }
            else
            {
                if (paths != null && paths.Any())
                {
                    query = paths.Aggregate(query, (current, path) => current.Include(path));
                }

                IOrderedQueryable<T> orderedQueryable;
                if (orderBy == null)
                {
                   // orderedQueryable = query.OrderBy(x=>x);
                }
                else
                {
                    orderedQueryable = query.Apply(orderBy as OrderByInfo<T>[] ?? orderBy.ToArray());
                    query = orderedQueryable;
                }

                //if (orderedQueryable == null)
                //{
                //    result = new PagedResult<T>(new List<T>(), page, size, 0);
                //}
                //else
                //{
                    if (fetchAll)
                    {
                        var data = await query.ToListAsync();
                        var count = data.Count();
                        result = new PagedResult<T>(data, 1, count, count);
                    }
                    else
                    {
                        var data = await query.Skip((page - 1) * size).Take(size).ToListAsync();
                        result = new PagedResult<T>(data, page, size, totalRecords);
                    }
                //}
            }
            return result;
        }

        public static IPagedResult<T> Fetch<T, TDbContext>(this TDbContext db, Expression<Func<T, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<T>> orderBy, Expression<Func<T, dynamic>>[] paths, bool readOnly = false) where T : class where TDbContext : DbContext
        {
            var fetchAll = (page == -1 && size == -1);
            IPagedResult<T> result;

            var query = (readOnly) ? db.Set<T>().AsNoTracking().Where(predicate) : db.Set<T>().Where(predicate);
            var totalRecords = (fetchAll) ? 0 : query.Count();
            var totalPages = (fetchAll) ? 1 : (int)Math.Ceiling((double)totalRecords / size);
            if (page > totalPages)
            {
                page = totalPages;
            }
            if (totalRecords == 0)
            {
                result = new PagedResult<T>(new List<T>(), page, size, 0);
            }
            else
            {
                if (paths != null && paths.Any())
                {
                    query = paths.Aggregate(query, (current, path) => current.Include(path));
                }

                IOrderedQueryable<T> orderedQueryable = query.Apply(orderBy as OrderByInfo<T>[] ?? orderBy.ToArray());

                if (orderedQueryable == null)
                {
                    result = new PagedResult<T>(new List<T>(), page, size, 0);
                }
                else
                {
                    if (fetchAll)
                    {
                        var data = orderedQueryable.ToList();
                        var count = data.Count();
                        result = new PagedResult<T>(data, 1, count, count);
                    }
                    else
                    {
                        var data = orderedQueryable.Skip((page - 1) * size).Take(size).ToList();
                        result = new PagedResult<T>(data, page, size, totalRecords);
                    }
                }
            }
            return result;
        }
    }

    internal static class IQueryRepositoryExtensions
    {
        public static async Task<IQueryResult<TEntity>> FindAsync<TEntity>(this IQueryRepository<TEntity> repository, EntityQueryRequest<TEntity> request) where TEntity : class
        {
            IQueryResult<TEntity> result = null;
            switch (request.SizeType)
            {
                case RequestSizeType.Scalar:
                    var hit = await repository.GetAsync(request.Predicate, request.Paths);
                    result = new ScalarQueryResult<TEntity>(hit);
                    break;

                case RequestSizeType.Page:
                    var page = await repository.FindAsync(request.Predicate, request.Page, request.Size, request.OrderBy, request.Paths);
                    result = new CollectionQueryResult<TEntity>(page.Records, page.Page, page.PageSize, page.TotalRecords);
                    break;

                case RequestSizeType.Collection:
                    var collection = await repository.FindAsync(request.Predicate, request.OrderBy, request.Paths);
                    result = new CollectionQueryResult<TEntity>(collection.Records, collection.Page, collection.PageSize, collection.TotalRecords);
                    break;

                default:
                    result = new FailQueryResult<TEntity>($"Can not support {request.SizeType} request;");
                    break;
            }
            return result;
        }
    }
}