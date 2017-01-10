namespace Borg.Infra.Storage
{
    public interface IVersionSpec
    {
        int Version { get; }
        IFileSpec FileSpec { get; }
    }
}