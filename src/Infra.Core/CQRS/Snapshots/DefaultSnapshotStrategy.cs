namespace Borg.Infra.CQRS
{
    public class DefaultSnapshotStrategy : IntervalSnapshotStrategy
    {
        public DefaultSnapshotStrategy() : base(100)
        {
        }
    }
}