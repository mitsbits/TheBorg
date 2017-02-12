namespace Borg.Infra.Relational
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
    }
}