namespace Borg.Infra.Storage.Assets
{
    public interface IVersionSpec
    {
        int Version { get; }
        IFileSpec FileSpec { get; }
    }
}