using hackernews;
using Moq;
using NUnit.Framework;

namespace Tests
{
    public class ScraperTest
    {
        private Mock<IHttp> http = new Mock<IHttp>();

        HackerNewsScraper scraper;

        [SetUp]
        public void Setup()
        {
            scraper = new HackerNewsScraper(http.Object);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}