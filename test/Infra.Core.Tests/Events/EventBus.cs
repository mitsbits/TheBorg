using Borg.Infra.CQRS;
using Moq;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Infra.CQRS.Tests.Events
{
    public class EventBusTests
    {
        private InMemoryPublisher _inMemoryPublisher;

        private EventTests.DummyEvent _eventA;
        private EventTests.DummyEvent _eventB;
        private DomainEventTests.DummyDomainEvent _domainEventA;
        private DomainEventTests.DummyDomainEvent _domainEventB;
        private string _eventKeyA = "xxx-xxxx-xxxx-xx";
        private string _eventKeyB = "ooo-oooo-oooo-oo";

        private Mock<IHandlesEvent<EventTests.DummyEvent>> _eventHandler;
        private Mock<IHandlesEvent<DomainEventTests.DummyDomainEvent>> _domainEventHandler;

        public EventBusTests()
        {
            _eventA = new EventTests.DummyEvent();
            _eventB = new EventTests.DummyEvent();
            _domainEventA = new DomainEventTests.DummyDomainEvent(_eventKeyA, 1);
            _domainEventB = new DomainEventTests.DummyDomainEvent(_eventKeyB, 2);

            _eventHandler = new Mock<IHandlesEvent<EventTests.DummyEvent>>();
            _eventHandler.Setup(x => x.Handle(_eventA)).Returns(Task.CompletedTask);
            _eventHandler.Setup(x => x.Handle(_eventB)).Returns(Task.CompletedTask);
            _domainEventHandler = new Mock<IHandlesEvent<DomainEventTests.DummyDomainEvent>>();
            _domainEventHandler.Setup(x => x.Handle(_domainEventA)).Returns(Task.CompletedTask);
            _domainEventHandler.Setup(x => x.Handle(_domainEventB)).Returns(Task.CompletedTask);
        }

        [Fact]
        public void check_that_we_can_register_event_handlers_to_the_in_memory_publisher()
        {
            Should.NotThrow(() =>
            {
                _inMemoryPublisher = new InMemoryPublisher();
                Action<EventTests.DummyEvent> e1 = @event => _eventHandler.Object.Handle(@event);
                Action<DomainEventTests.DummyDomainEvent> e2 = @event => _domainEventHandler.Object.Handle(@event);
                _inMemoryPublisher.RegisterHandler(e1);
                _inMemoryPublisher.RegisterHandler(e2);
            });
        }

        [Fact]
        public void check_that_a_publisher_dispatches_events()
        {
            _inMemoryPublisher = new InMemoryPublisher();
            Action<EventTests.DummyEvent> e1 = @event => _eventHandler.Object.Handle(@event).ConfigureAwait(false);
            Action<DomainEventTests.DummyDomainEvent> e2 = @event => _domainEventHandler.Object.Handle(@event).ConfigureAwait(false);
            _inMemoryPublisher.RegisterHandler(e1);
            _inMemoryPublisher.RegisterHandler(e2);
            Should.NotThrow(async () =>
            {
                await _inMemoryPublisher.Publish(_eventA);
                await _inMemoryPublisher.Publish(_eventB);
                await _inMemoryPublisher.Publish(_domainEventA);
                await _inMemoryPublisher.Publish(_domainEventB);

                _eventA = new EventTests.DummyEvent();
                _domainEventA.SetVersionAndResetTimestamp(5);

                await _inMemoryPublisher.Publish(_eventA);
                await _inMemoryPublisher.Publish(_domainEventA);
            });
            _inMemoryPublisher.TimesRun.ShouldBe(6);
        }
    }
}