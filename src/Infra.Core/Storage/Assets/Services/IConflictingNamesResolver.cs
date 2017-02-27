using System.Threading.Tasks;

namespace Borg.Infra.Storage.Assets
{
    public interface IConflictingNamesResolver
    {
        Task<string> Resolve(string filename);
    }
}