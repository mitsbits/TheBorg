using System;

namespace Borg.Infra.Storage
{
    public class VersionNotFoundException<TKey> : Exception
    {
        public VersionNotFoundException(TKey id, int version) : base($"Version {version} not found for Asset with Id {id}")
        {
        }
    }
}