using System;
using Borg.Infra.BuildingBlocks;
using Borg.Infra.Caching;
using Borg.Infra.CQRS;
using Borg.Infra.Relational;
using Infra.Core.Relational;
using Mehdime.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Borg.Infra.EF6
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
            PreProcess -= PreProcessInteral;
            PostProcess -= PostProcessInteral;
        }
    }
}

namespace Borg
{
    internal static partial class IQueryRepositoryExtensions
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