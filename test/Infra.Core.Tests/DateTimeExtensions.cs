using Shouldly;
using System;
using Xunit;

namespace Borg.Infra.Core.Tests
{
    public class DateTimeExtensionsTests
    {
        [Theory]
        [InlineData("2016/12/29 20:21", "2016/12/29 20:25", "00:05")]
        [InlineData("2016/12/29 20:21", "2016/12/29 20:30", "00:10")]
        [InlineData("2016/12/29 20:21", "2016/12/29 20:30", "00:15")]
        [InlineData("2016/12/29 20:21", "2016/12/29 20:40", "00:20")]
        [InlineData("2016/12/29 20:21", "2016/12/29 20:30", "00:30")]
        [InlineData("2016/12/29 20:21", "2016/12/29 21:00", "00:45")]
        [InlineData("2016/12/29 20:21", "2016/12/29 21:00", "01:00")]
        [InlineData("2016/12/29 20:21", "2016/12/29 22:00", "02:00")]
        [InlineData("2016/12/29 20:21", "2016/12/30 00:00", "04:00")]
        public void test_round_up_date_extension_method(string source, string target, string interval)
        {
            DateTime s = DateTime.Parse(source);
            DateTime t = DateTime.Parse(target);
            TimeSpan i;
            TimeSpan.TryParse(interval, out i);

            s.RoundUp(i).ShouldBe(t);
        }

        [Theory]
        [InlineData("2016/12/29 20:21", "2016/12/29 20:20", "00:05")]
        [InlineData("2016/12/29 20:21", "2016/12/29 20:20", "00:10")]
        [InlineData("2016/12/29 20:21", "2016/12/29 20:15", "00:15")]
        [InlineData("2016/12/29 20:21", "2016/12/29 20:20", "00:20")]
        [InlineData("2016/12/29 20:21", "2016/12/29 20:00", "00:30")]
        [InlineData("2016/12/29 20:21", "2016/12/29 20:15", "00:45")]
        [InlineData("2016/12/29 20:21", "2016/12/29 20:00", "01:00")]
        [InlineData("2016/12/29 20:21", "2016/12/29 20:00", "02:00")]
        [InlineData("2016/12/29 20:21", "2016/12/29 20:00", "04:00")]
        public void test_round_up_down_extension_method(string source, string target, string interval)
        {
            DateTime s = DateTime.Parse(source);
            DateTime t = DateTime.Parse(target);
            TimeSpan i;
            TimeSpan.TryParse(interval, out i);

            s.RoundDown(i).ShouldBe(t);
        }
    }
}