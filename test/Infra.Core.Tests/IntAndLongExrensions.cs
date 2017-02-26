using Shouldly;
using Xunit;

namespace Borg.Infra.Core.Tests
{
    public class IntAndLongExrensionsTests
    {
        [Theory]
        [InlineData(0, "0 B")]
        [InlineData(3072, "3 KB")]
        [InlineData(3413, "3,3 KB")]
        [InlineData(4194304, "4 MB")]
        public void test_bytes_to_string_extension_method_default_format(long source, string target)
        {
            source.BytesDisplay().ShouldBe(target);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(2, 0, 5)]
        [InlineData(3, 5, 5)]
        [InlineData(4, 5, 5)]
        [InlineData(7, 5, 5)]
        [InlineData(9, 10, 5)]
        [InlineData(12, 10, 5)]
        [InlineData(16, 15, 5)]
        [InlineData(25, 0, 50)]
        [InlineData(26, 50, 50)]
        [InlineData(75, 100, 50)]
        [InlineData(76, 100, 50)]
        [InlineData(50, 0, 100)]
        [InlineData(51, 100, 100)]
        [InlineData(149, 100, 100)]
        [InlineData(150, 200, 100)]
        [InlineData(250, 200, 100)]
        [InlineData(251, 300, 100)]
        public void test_round_off_extension_method(int source, int target, int interval)
        {
            source.RoundOff(interval).ShouldBe(target);
        }
    }
}