using System;
using System.Threading.Tasks;

namespace Borg.Infra.Storage.Assets
{
    public interface IAssetMetadataStorage<TKey> where TKey : IEquatable<TKey>
    {
        Task Add(IAssetSpec<TKey> spec);

        Task Activate(TKey id);

        Task Deactivate(TKey id);

        Task Remove(TKey id);

        Task AddVersion(TKey id, IVersionSpec spec);

        Task<IAssetSpec<TKey>> Get(TKey id);

        Task<IVersionSpec> GetVersion(TKey id, int version);
    }
}