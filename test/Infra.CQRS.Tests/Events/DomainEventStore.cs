using Borg.Infra.CQRS;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Infra.CQRS.Tests.Events
{
    public class DomainEventStoreTests
    {
        private InMemoryEventStore<string> _inMemoryEventStore;

        private string _aggregateKeyA = "xxx-xxxx-xxxx-xx";
        private string _aggregateKeyB = "ooo-oooo-oooo-oo";

        public DomainEventStoreTests()
        {
            _inMemoryEventStore = new InMemoryEventStore<string>(new InMemoryPublisher());
        }

        [Fact]
        public void check_that_we_save_events()
        {
            PopulateDb();
            _inMemoryEventStore.AggregateKeys.Count().ShouldBe(2);
        }

        [Fact]
        public void check_that_we_can_save_multiple_events_for_the_same_key()
        {
            PopulateDb();
            _inMemoryEventStore.AggregateKeys.Count().ShouldBe(2);
        }

        [Fact]
        public async Task check_that_we_can_retrieve_events_for_aggregate_key()
        {
            PopulateDb();
            _inMemoryEventStore.AggregateKeys.Count().ShouldBe(2);
            var stream = await _inMemoryEventStore.Get<DummyAggregateA>(_aggregateKeyA);
            stream.ShouldNotBeNull();
            stream.Count().ShouldBe(9);
            stream = await _inMemoryEventStore.Get<DummyAggregateB>(_aggregateKeyB);
            stream.ShouldNotBeNull();
            stream.Count().ShouldBe(9);
        }

        private void PopulateDb()
        {
            _inMemoryEventStore.Clear();
            var events = new[]
            {
                new DomainEventTests.DummyDomainEvent(_aggregateKeyA, 1),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyA, 2),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyA, 3),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyA, 4),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyA, 5),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyA, 6),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyA, 7),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyA, 8),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyA, 9),
            };
            Should.NotThrow(async () => { await _inMemoryEventStore.Save<DummyAggregateA>(events); });
            events = new[]
            {
                new DomainEventTests.DummyDomainEvent(_aggregateKeyB, 1),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyB, 2),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyB, 3),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyB, 4),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyB, 5),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyB, 6),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyB, 7),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyB, 8),
                new DomainEventTests.DummyDomainEvent(_aggregateKeyB, 9),
            };
            Should.NotThrow(async () => { await _inMemoryEventStore.Save<DummyAggregateB>(events); });
        }

        private class DummyAggregateA : AggregateRoot<string>
        {
        }

        private class DummyAggregateB : AggregateRoot<string>
        {
        }
    }
}