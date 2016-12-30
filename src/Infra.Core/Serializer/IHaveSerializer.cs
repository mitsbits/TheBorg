namespace Borg.Infra.Core
{
    public interface IHaveSerializer
    {
        ISerializer Serializer { get; }
    }
}