namespace Borg.Infra.Relational
{
    public interface IContextAwareUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets the underlying database context
        /// </summary>
        object GetContext();
    }
}