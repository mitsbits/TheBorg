using System;

namespace Borg.Framework.Services.Navigation.Contracts
{
    public interface IHasParent<out TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; }

        TKey ParentId { get; }
    }
}