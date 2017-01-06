using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Borg.Infra.EF6
{
    public class DiscoveryDbContextSpec
    {
        public bool TryToCreateSequence { get; set; } = false;
        public string SchemaName { get; set; } = "";
        public string ConnectionStringOrName { get; set; } = "default";
        public IEnumerable<IAssemblyProvider> AssemblyProviders { get; set; }

        public IEnumerable<IAssemblyProvider> GetProviders(bool createDefaultIfEmpty = true, Func<Assembly, bool> createDefaultIfEmptyFilter = null)
        {
            if (AssemblyProviders != null && AssemblyProviders.Any()) return AssemblyProviders;
            return (createDefaultIfEmpty) ? new[] { new CurrentContextAssemblyProvider(createDefaultIfEmptyFilter) } : new IAssemblyProvider[0];
        }
    }
}