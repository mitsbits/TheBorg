namespace Borg.Infra.CQRS
{
    public interface IHandlesMessage<in T> where T : IMessage
    {
        void Handle(T message);
    }
}