using System;
using System.Threading.Tasks;

namespace Borg.Infra.Storage.Assets
{
    public interface IAssetService<TKey> where TKey : IEquatable<TKey>
    {
        Task<IAssetSpec<TKey>> Create(string name, byte[] content, string fileName, AssetState state, string contentType = "");

        Task Suspend(TKey id);

        Task Acivate(TKey id);

        Task Delete(TKey id);

        Task<IAssetSpec<TKey>> AddNewVersion(TKey id, byte[] content, string fileName, string contentType = "");
    }
}