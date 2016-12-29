using System.Threading.Tasks;

namespace Borg.Infra.Relational.EF6
{
    internal interface IRepositoryThatCommits
    {
        int SaveChanges();

        Task<int> SaveChangesAsync();
    }
}