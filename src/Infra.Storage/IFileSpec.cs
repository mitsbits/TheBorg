using System;

namespace Borg.Infra.Storage
{
    public interface IFileSpec<out TKey> : IFileSpec where TKey : IEquatable<TKey>
    {
        TKey Id { get; }
    }
}