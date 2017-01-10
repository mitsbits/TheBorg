namespace Borg.Infra.Storage
{
    public class VersionSpec : IVersionSpec
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