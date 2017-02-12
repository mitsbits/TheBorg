

namespace Borg.Infra.Relational
{
    public interface IUowRepository
    {
        void SetUnitOfWork(IUnitOfWork unitOfWork);
    }
}