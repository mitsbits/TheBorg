using Borg.Infra.CQRS;

namespace Borg.Infra.EF6
{
    public abstract class SequenceEntity : Entity<int>, ISequenceEntity
    {
    }
}