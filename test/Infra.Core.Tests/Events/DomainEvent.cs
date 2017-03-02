using Borg.Infra.CQRS;
using Shouldly;
using System;
using Xunit;

namespace Infra.CQRS.Tests.Events
{
    public class DomainEventTests
    {
        private DummyDomainEvent _event;
        private string _eventKey = "xxx-xxxx-xxxx-xx";

        public DomainEventTests()
        {
        }

        [Fact]
        public void check_that_event_has_a_timetamp()
        {
            var clock = DateTimeOffset.UtcNow;
            _event = new DummyDomainEvent();
            var clock2 = DateTimeOffset.UtcNow;
            _event.Timestamp.ShouldNotBeNull();
            clock.ShouldBeLessThanOrEqualTo(_event.Timestamp);
            clock2.ShouldBeGreaterThanOrEqualTo(_event.Timestamp);
        }

        [Fact]
        public void check_that_we_can_reset_timestamp_and_increase_version()
        {
            _event = new DummyDomainEvent(_eventKey, 1);
            _event.Version.ShouldBe(1);
            var oldTimestamp = _event.Timestamp;
            _event.SetVersionAndResetTimestamp(3);
            _event.Version.ShouldBe(3);
            _event.Timestamp.ShouldBeGreaterThanOrEqualTo(oldTimestamp);
        }

        internal class DummyDomainEvent : DomainEvent<string>
        {
            public DummyDomainEvent() : base()
            {
            }

            public DummyDomainEvent(string id, int version) : base(id, version)
            {
            }
        }
    }
}