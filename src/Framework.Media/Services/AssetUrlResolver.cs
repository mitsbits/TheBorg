using Borg.Framework.System;
using Borg.Infra.Storage.Assets;

namespace Borg.Framework.Media.Services
{
    public class AssetUrlResolver : IAssetUrlResolver
    {
        private readonly BorgSettings _settings;
        private readonly string _appRoot;

        public AssetUrlResolver(BorgSettings settings)
        {
            _settings = settings;
            _appRoot = _settings.Backoffice.Application.BaseUrl;
        }

        public string ResolveSourceUrlFromFullPath(string fullpath)
        {
            var root = _settings.Backoffice.Application.Storage.SharedFolder;
            var relative = fullpath.Replace(root, string.Empty).Replace("\\", "/");
            return $"{_appRoot.TrimEnd('/')}/{relative.TrimStart('/')}";
        }
    }
}