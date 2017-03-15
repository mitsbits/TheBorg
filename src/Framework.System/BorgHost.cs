using System.Collections.Generic;
using System.Linq;

namespace Borg.Framework.System
{
    public class BorgHost : IBorgHost
    {
        public BorgHost(IEnumerable<IBorgPlugin> registeredPlugins)
        {
            RegisteredPlugins = registeredPlugins?.ToArray();
        }

        public IBorgPlugin[] RegisteredPlugins { get; }
    }
}