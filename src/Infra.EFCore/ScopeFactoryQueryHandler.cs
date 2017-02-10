using Borg.Infra.BuildingBlocks;
using Borg.Infra.Caching;
using Borg.Infra.CQRS;
using Borg.Infra.Relational;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Borg.Infra.EFCore
{
    internal abstract class ScopeFactoryQueryHandler<TRequest> : IHandlesQueryRequest<TRequest> where TRequest : IQueryRequest
    {
        protected ScopeFactoryQueryHandler(IDbContextScopeFactory dbContextScopeFactory)
        {
            ScopeFactory = dbContextScopeFactory;
        }

        protected IDbContextScopeFactory ScopeFactory { get; }

        public abstract Task<IQueryResult> Execute(TRequest message);
    }

    internal delegate Task<IQueryResult<TEntity>> PreProcessQueryRequestAsyncHandler<TEntity>(EntityQueryRequest<TEntity> request) where TEntity : class;

    internal delegate Task PostProcessQueryRequestAsyncHandler<TEntity>(EntityQueryRequest<TEntity> request, IQueryResult<TEntity> result) where TEntity : class;

    internal abstract class ScopeFactoryEntityQueryHandler<TRequest, TEntity> : IHandlesQueryRequest<TRequest, TEntity> where TRequest : EntityQueryRequest<TEntity> where TEntity : class
    {
        protected ScopeFactoryEntityQueryHandler(IDbContextScopeFactory dbContextScopeFactory, IQueryRepository<TEntity> repository)
        {
            ScopeFactory = dbContextScopeFactory;
            Repository = repository;
        }

        protected PreProcessQueryRequestAsyncHandler<TEntity> PreProcess { get; set; } = null;
        protected PostProcessQueryRequestAsyncHandler<TEntity> PostProcess { get; set; } = null;
        protected IDbContextScopeFactory ScopeFactory { get; }
        private IQueryRepository<TEntity> Repository { get; }

        public virtual async Task<IQueryResult<TEntity>> Execute(TRequest message)
        {
            IQueryResult<TEntity> result = null;
            var preInvoke = PreProcess?.Invoke(message);
            if (preInvoke != null) result = await preInvoke;
            if (result != null) return result;
            using (ScopeFactory.CreateReadOnly())
            {
                result = await Repository.FindAsync(message);
            }
            var postInvoke = PostProcess?.Invoke(message, result);
            if (postInvoke != null) await postInvoke;
            return result;
        }
    }

    internal abstract class CachedScopeFactoryEntityQueryHandler<TRequest, TEntity> : ScopeFactoryEntityQueryHandler<TRequest, TEntity>, IDisposable where TRequest : EntityQueryRequest<TEntity> where TEntity : class
    {
        private IDepedencyCacheClient CacheClient { get; }

        protected CachedScopeFactoryEntityQueryHandler(IDbContextScopeFactory dbContextScopeFactory, IQueryRepository<TEntity> repository, IDepedencyCacheClient cacheClient) : base(dbContextScopeFactory, repository)
        {
            CacheClient = cacheClient;

            PreProcess += PreProcessInteral;
            PostProcess += PostProcessInteral;
        }

        private async Task<IQueryResult<TEntity>> PreProcessInteral(EntityQueryRequest<TEntity> request)
        {
            var cacheKey = (request as ICanProduceCacheKey)?.CacheKey;
            if (string.IsNullOrWhiteSpace(cacheKey)) return null;
            var cached = await CacheClient.GetAsync<IQueryResult<TEntity>>(cacheKey);
            return cached.HasValue ? cached.Value : null;
        }

        private async Task PostProcessInteral(EntityQueryRequest<TEntity> request, IQueryResult<TEntity> result)
        {
            if (result == null) return;
            var cacheKey = (request as ICanProduceCacheKey)?.CacheKey;
            if (string.IsNullOrWhiteSpace(cacheKey)) return;
            await CacheClient.SetAsync(cacheKey, result, (request as ICanProduceCacheExpiresIn)?.ExpiresIn);
            if (typeof(TEntity).GetInterface(nameof(IHavePartitionedKey)) != null)
                await CacheClient.Add<TEntity>(cacheKey, result.Records.Cast<IHavePartitionedKey>().Select(x => x.Key).ToArray());
        }

        public virtual void Dispose()
        {
            if (PreProcess != null) PreProcess -= PreProcessInteral;
            if (PostProcess != null) PostProcess -= PostProcessInteral;
        }
    }
}

