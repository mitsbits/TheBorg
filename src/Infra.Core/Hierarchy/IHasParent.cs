namespace Borg.Infra.Hierarchy
{
    public interface IHasParent<out TKey> : IHasParent
    {
        TKey Id { get; }

        TKey ParentId { get; }
    }

    public interface IHasParent
    {
        int Depth { get; }
    }
}