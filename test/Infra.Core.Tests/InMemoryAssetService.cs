using Borg;
using Borg.Infra.Storage;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Infra.Storage.Tests
{
    public class InMemoryAssetService
    {
        private readonly IFileStorage _storage;
        private readonly IUniqueKeyProvider<Guid> _keyProvider;
        private readonly IConflictingNamesResolver _namesResolver;
        private readonly IAssetService<Guid> _service;

        public InMemoryAssetService()
        {
            _storage = new InMemoryFileStorage();
            _keyProvider = new GuidKeyProvider();
            _namesResolver = new DefaultConflictingNamesResolver();
            _service = new InMemoryAssetService<Guid>(_storage, _keyProvider, _namesResolver);
        }

        [Fact]
        public async Task test()
        {
            var path = @"K:\Users\mitsbits\Desktop\peiraiws-ceo-708.jpg";
            byte[] data = File.ReadAllBytes(path);
            var info = new FileInfo(path);
            var asset = await _service.Create(info.Name, data, info.Name, AssetState.Active);
            path = @"K:\Users\mitsbits\Desktop\giphy.gif";
            data = File.ReadAllBytes(path);
            info = new FileInfo(path);
            await _service.AddNewVersion(asset.Id, data, info.Name);
            await _service.Suspend(asset.Id);
            await _service.Acivate(asset.Id);
        }
    }
}