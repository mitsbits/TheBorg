using System.Collections.Generic;

namespace Borg.Infra
{
    public interface IHaveAssemblyScanner
    {
        IEnumerable<IAssemblyProvider> Providers { get; }
    }
}