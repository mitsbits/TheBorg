using System.Reflection;

namespace Borg.Infra
{
    public interface IAssemblyProvider
    {
        Assembly[] Assemblies();
    }
}