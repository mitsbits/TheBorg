using System.Threading.Tasks;

namespace Borg.Infra.Storage
{
    public interface IConflictingNamesResolver
    {
        Task<string> Resolve(string filename);
    }
}