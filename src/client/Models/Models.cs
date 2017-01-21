using Borg.Infra.BuildingBlocks;
using Borg.Infra.EF6;
using Borg.Infra.EF6.Discovery;
using Borg.Infra.Messaging;
using Borg.Infra.Relational;
using Mehdime.Entity;
using System.Collections.Generic;

namespace Borg.Client.Models
{
    [MapEntity]
    public class Category
    {
        public int Id { get; set; }
        public string Display { get; set; }
    }

    [MapEntity]
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string[] Body { get; set; }
        public UrlInfo UrlInfo { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }

    public class BlogsDbContext : DiscoveryDbContext
    {
        public BlogsDbContext(DiscoveryDbContextSpec spec) : base(spec)
        {
        }
    }

    public class QueryRepository<TEntity> : ContextScopedQueryRepository<TEntity, BlogsDbContext> where TEntity : class
    {
        public QueryRepository(IAmbientDbContextLocator ambientDbContextLocator) : base(ambientDbContextLocator)
        {
        }
    }

    public class CrudRepository<TEntity> : ContextScopedReadWriteRepository<TEntity, BlogsDbContext>, ICRUDRespoditory<TEntity> where TEntity : class
    {
        public CrudRepository(IAmbientDbContextLocator ambientDbContextLocator) : base(ambientDbContextLocator)
        {
        }
    }

    public class AppBroadcaster : BroadcasterBase
    {
        public AppBroadcaster(IEnumerable<IMessagePublisher> publishers) : base(publishers)
        {
        }
    }
}