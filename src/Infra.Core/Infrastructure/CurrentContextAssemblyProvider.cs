using Borg.Infra.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Borg.Infra
{
    public class CurrentContextAssemblyProvider : IAssemblyProvider
    {
        private readonly Func<Assembly, bool> _predicate;

        public Lazy<List<Assembly>> _asmbls = new Lazy<List<Assembly>>(() =>

        {
            var refs = Assembly.GetEntryAssembly().GetReferencedAssemblies().ToArray();
            foreach (var name in refs)
            {
                if (AppDomain.CurrentDomain.GetAssemblies().All(a => a.FullName != name.FullName))
                {
                    if (name.Name.StartsWith("Ubik"))
                        LoadReferencedAssembly(Assembly.Load(name));
                }
            }
            return new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
        });

        public CurrentContextAssemblyProvider(Func<Assembly, bool> predicate = null)
        {
            _predicate = predicate;
        }

        private static void LoadReferencedAssembly(Assembly assembly)
        {
            var refs = assembly.GetReferencedAssemblies().ToArray();
            foreach (AssemblyName name in refs)
            {
                if (AppDomain.CurrentDomain.GetAssemblies().All(a => a.FullName != name.FullName))
                {
                    LoadReferencedAssembly(Assembly.Load(name));
                }
            }
        }

        public Assembly[] Assemblies()
        {
            return (_predicate == null) ? _asmbls.Value.ToArray() : _asmbls.Value.Where(_predicate).ToArray();
        }
    }
}