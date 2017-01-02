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
            _event.TimeStamp.ShouldNotBeNull();
            clock.ShouldBeLessThanOrEqualTo(_event.TimeStamp);
            clock2.ShouldBeGreaterThanOrEqualTo(_event.TimeStamp);
        }

        [Fact]
        public void check_that_we_can_reset_timestamp_and_increase_version()
        {
            _event = new DummyDomainEvent(_eventKey, 1);
            _event.Version.ShouldBe(1);
            var oldTimestamp = _event.TimeStamp;
            _event.SetVersionAndResetTimestamp(3);
            _event.Version.ShouldBe(3);
            _event.TimeStamp.ShouldBeGreaterThanOrEqualTo(oldTimestamp);
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