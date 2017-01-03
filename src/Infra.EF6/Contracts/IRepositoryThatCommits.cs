using System.Threading.Tasks;

namespace Borg.Infra.EF6
{
    internal interface IRepositoryThatCommits
    {
        int SaveChanges();

        Task<int> SaveChangesAsync();
    }
}