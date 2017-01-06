using Borg.Infra.CQRS;
using System;

namespace Borg.Infra.BuildingBlocks
{
    public class PartitionedKey : ValueObject<PartitionedKey>
    {
        protected PartitionedKey()
        {
        }

        public PartitionedKey(string partition, string key = "")
        {
            if (string.IsNullOrWhiteSpace(partition)) throw new ArgumentNullException(nameof(partition));
            Partition = partition;
            Key = key;
        }

        public string Partition { get; protected set; }
        public string Key { get; protected set; }

        public override string ToString()
        {
            return $"{Partition}{Key}";
        }
    }
}