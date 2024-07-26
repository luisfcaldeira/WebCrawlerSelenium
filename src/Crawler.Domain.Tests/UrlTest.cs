using Crawler.Domain.Entities;

namespace Crawler.Domain.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestEquals()
        {
            var url = new Url("www.domain.com");

            Assert.That(url.Equals(new Url("www.domain.com")));
            Assert.That(!url.Equals(new Url("www.different.com")));
        }
    }
}