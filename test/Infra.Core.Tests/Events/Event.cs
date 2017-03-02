using Borg.Infra.CQRS;
using Shouldly;
using System;
using Xunit;

namespace Infra.CQRS.Tests.Events
{
    public class EventTests
    {
        private DummyEvent _event;

        public EventTests()
        { }

        [Fact]
        public void check_that_event_has_a_timetamp()
        {
            var clock = DateTimeOffset.UtcNow;
            _event = new DummyEvent();
            var clock2 = DateTimeOffset.UtcNow;
            _event.Timestamp.ShouldNotBeNull();
            clock.ShouldBeLessThanOrEqualTo(_event.Timestamp);
            clock2.ShouldBeGreaterThanOrEqualTo(_event.Timestamp);
        }

        internal class DummyEvent : Event
        {
        }
    }
}