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
            _event.TimeStamp.ShouldNotBeNull();
            clock.ShouldBeLessThanOrEqualTo(_event.TimeStamp);
            clock2.ShouldBeGreaterThanOrEqualTo(_event.TimeStamp);
        }

        internal class DummyEvent : Event
        {
        }
    }
}