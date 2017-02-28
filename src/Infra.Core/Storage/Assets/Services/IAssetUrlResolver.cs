namespace Borg.Infra.Storage.Assets
{
    public interface IAssetUrlResolver
    {
        string ResolveSourceUrlFromFullPath(string fullpath);
    }
}