using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Borg.Infra
{
    public class DirectoryAssemblyProvider : IAssemblyProvider
    {
        private readonly Func<Assembly, bool> _predicate;

        private readonly List<Assembly> _asmbls = new List<Assembly>();

        public DirectoryAssemblyProvider(string path, Func<Assembly, bool> predicate = null)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            var directory = new DirectoryInfo(path);
            if (!directory.Exists) throw new ArgumentOutOfRangeException(nameof(path));
            var files = Directory.GetFiles(directory.FullName, "*.dll", SearchOption.AllDirectories);
            _asmbls.AddRange(files.Select(x => Assembly.Load(new AssemblyName(x))));
            _predicate = predicate;
        }

        public Assembly[] Assemblies()
        {
            return (_predicate == null) ? _asmbls.ToArray() : _asmbls.Where(_predicate).ToArray();
        }
    }
}