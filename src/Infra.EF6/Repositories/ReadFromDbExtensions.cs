using Borg.Infra.Relational;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                        var data = await orderedQueryable.ToListAsync();
                        var count = data.Count();
                        result = new PagedResult<T>(data, 1, count, count);
                    }
                    else
                    {
                        var data = await orderedQueryable.Skip((page - 1) * size).Take(size).ToListAsync();
                        result = new PagedResult<T>(data, page, size, totalRecords);
                    }
                }
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
}