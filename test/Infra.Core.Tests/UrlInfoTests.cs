using Borg.Infra.BuildingBlocks;
using Shouldly;
using Xunit;

namespace Borg.Infra.Core.Tests
{
    public class UrlInfoTests
    {
        public UrlInfoTests()
        {
        }

        [Theory]
        [InlineData("http://news.in.gr/greece/article/?aid=1500123825", "")]
        [InlineData("https://github.com/mitsbits/Ubik/commit/45d44fe6f55fe34e9df0435ba5320781f4f85312", "")]
        [InlineData("https://www.google.gr/webhp?sourceid=chrome-instant&ion=1&espv=2&ie=UTF-8#q=c%23%20get%20domain%20from%20url", "google.gr")]
        public void test(string source, string domain)
        {
            Should.NotThrow(() =>
            {
                var d = new UrlInfo(source, domain);
                var s = d.SubDomain;
            });
        }
    }
}