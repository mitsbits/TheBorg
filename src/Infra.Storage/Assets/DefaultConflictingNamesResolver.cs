using System;
using System.IO;
using System.Threading.Tasks;

namespace Borg.Infra.Storage
{
    public class DefaultConflictingNamesResolver : IConflictingNamesResolver
    {
        public Task<string> Resolve(string filename)
        {
            return
                Task.FromResult(
                    $"{Path.GetFileNameWithoutExtension(filename)}.{DateTime.UtcNow.Ticks}{Path.GetExtension(filename)}");
        }
    }
}