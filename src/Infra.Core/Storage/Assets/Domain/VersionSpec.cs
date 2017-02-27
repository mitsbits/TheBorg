namespace Borg.Infra.Storage.Assets
{
    internal class VersionSpec : IVersionSpec
    {
        public VersionSpec(int version, IFileSpec fileSpec)
        {
            Version = version;
            FileSpec = fileSpec;
        }

        public int Version { get; }
        public IFileSpec FileSpec { get; }
    }
}