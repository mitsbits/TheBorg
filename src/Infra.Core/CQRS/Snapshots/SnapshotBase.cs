using System;

namespace Borg.Infra.CQRS
{
    public abstract class SnapshotBase<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
        public int Version { get; set; }
    }
}