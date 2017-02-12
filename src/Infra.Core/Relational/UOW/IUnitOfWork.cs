using System;


namespace Borg.Infra.Relational
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Strongly typed repository resolution method using the IoC container
        /// </summary>
        /// <typeparam name="TRepository">
        /// Concrete repository type. 
        /// Should normally be an strong interface like ICustomerRepository.
        /// </typeparam>
        /// <returns>Requested repository resolved from IoC container.</returns>
        TRepository GetRepository<TRepository>() where TRepository : class, IUowRepository;


        /// <summary>
        /// Indicated that unit of work finished, either by calling Commit or Dispose.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// Persist all changes for given unit of work, also commiting the transaction
        /// if one is open and if this is the topmost transactional unit of work
        /// in the stack.
        /// </summary>
        void Commit();
    }

    public interface IScopeManager
    {
        void Complete(IUnitOfWork unitOfWork);
        void Remove(IUnitOfWork unitOfWork);
    }
}