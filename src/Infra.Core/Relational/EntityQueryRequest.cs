using Borg.Infra.Caching;
using Borg.Infra.CQRS;
using Borg.Infra.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Infra.Core.Relational
{
    public abstract class EntityQueryRequest<TEntity> : IQueryRequest<TEntity> where TEntity : class
    {
        protected EntityQueryRequest(Expression<Func<TEntity, bool>> predicate, int page, int size,
            IEnumerable<OrderByInfo<TEntity>> orderBy, params Expression<Func<TEntity, dynamic>>[] paths)
        {
            Predicate = predicate;
            Page = page;
            Size = size;
            OrderBy = orderBy;
            Paths = paths ?? new Expression<Func<TEntity, dynamic>>[0];
        }

        public Expression<Func<TEntity, bool>> Predicate { get; }
        public int Page { get; }
        public int Size { get; }
        public IEnumerable<OrderByInfo<TEntity>> OrderBy { get; }
        public Expression<Func<TEntity, dynamic>>[] Paths { get; }

        public virtual RequestSizeType SizeType => RequestSizeType.Collection;
    }

    public abstract class EntityCachedQueryRequest<TEntity> : EntityQueryRequest<TEntity>, ICanProduceCacheKey, ICanProduceCacheExpiresIn where TEntity : class
    {
        protected EntityCachedQueryRequest(TimeSpan? expiresin, Expression<Func<TEntity, bool>> predicate, int page, int size,
            IEnumerable<OrderByInfo<TEntity>> orderBy, params Expression<Func<TEntity, dynamic>>[] paths) : base(predicate, page, size, orderBy, paths)
        {
            _expiresin = expiresin;
        }

        protected virtual string GetCacheKey()
        {
            var sb = new StringBuilder($"H:{GetType().Name}");
            sb.Append('_');
            sb.Append($"E:{typeof(TEntity).Name}");
            sb.Append('_');
            sb.Append($"W:{Predicate.Body.ToString().Replace($"{Predicate.Parameters[0].Name}.", string.Empty).TrimStart('(').TrimEnd(')')}");
            sb.Append('_');
            sb.Append($"P:{Page}:{Size}");
            if (OrderBy.Any())
            {
                sb.Append('_');
                sb.Append($"O:{string.Join("|", OrderBy.Select(o => o.ToString()).ToArray())}");
            }
            if (Paths.Any())
            {
                sb.Append('_');
                sb.Append($"I:{string.Join("|", Paths.Select(e => ((MemberExpression)((UnaryExpression)e.Body).Operand).Member.Name).ToArray())}");
            }
            return sb.Replace(" ", string.Empty).ToString();
        }

        private string _cacheKey = null;
        string ICanProduceCacheKey.CacheKey => _cacheKey ?? (_cacheKey = GetCacheKey());

        private readonly TimeSpan? _expiresin;
        TimeSpan? ICanProduceCacheExpiresIn.ExpiresIn => _expiresin;
    }

    internal abstract class EntitiesScalarQueryRequest<TEntity> : EntityQueryRequest<TEntity> where TEntity : class
    {
        protected EntitiesScalarQueryRequest(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, dynamic>>[] paths) : base(predicate, 1, 1, null, paths)
        {
        }

        public override RequestSizeType SizeType => RequestSizeType.Scalar;
    }

    internal abstract class EntitiesPageQueryRequest<TEntity> : EntityQueryRequest<TEntity> where TEntity : class
    {
        protected EntitiesPageQueryRequest(Expression<Func<TEntity, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<TEntity>> orderBy, params Expression<Func<TEntity, dynamic>>[] paths) : base(predicate, page, size, orderBy, paths)
        {
        }

        public override RequestSizeType SizeType => RequestSizeType.Page;
    }

    internal abstract class EntitiesCollectionQueryRequest<TEntity> : EntityQueryRequest<TEntity> where TEntity : class
    {
        protected EntitiesCollectionQueryRequest(Expression<Func<TEntity, bool>> predicate, IEnumerable<OrderByInfo<TEntity>> orderBy, params Expression<Func<TEntity, dynamic>>[] paths) : base(predicate, -1, -1, orderBy, paths)
        {
        }

        public override RequestSizeType SizeType => RequestSizeType.Collection;
    }
}