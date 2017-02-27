using Borg.Infra.Relational;
using Borg.Infra.Storage;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace Borg.Framework.Media
{
    public interface IMediaService : IAssetService<int>, IDisposable
    {
        Task<IPagedResult<AssetSpec>> Assets(Expression<Func<AssetSpec, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<AssetSpec>> orderBy, params Expression<Func<AssetSpec, dynamic>>[] paths);

        Task AssetChangeName(int id, string name);

        Task AssetChangeState(int id, AssetState state);
    }
}