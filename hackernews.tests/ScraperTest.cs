using hackernews;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;

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
        public async Task Test_GetPostsJson_ReturnsExceptionInformationIfThrown()
        {
            http.Setup(m => m.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult("gibberish"))
                .Verifiable();

            var result = await scraper.GetPostsJson(5);

            Assert.IsTrue(result.StartsWith("Exception"));

            http.VerifyAll();
        }

        [Test]
        public async Task Test_GetPostsJson_ReturnsNPosts()
        {
            HackerNewsPostResponse post = new HackerNewsPostResponse
            {
                by = "test",
                title = "test",
                url = "http://www.google.com",
                score = 1,
                descendants = 1,
            };
            var response = JsonConvert.SerializeObject(post);

            http.SetupSequence(m => m.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult("[1,2,3]"))
                .Returns(Task.FromResult(response));

            var result = await scraper.GetPostsJson(1);
            var expected = "[\r\n  {\r\n    \"title\": \"test\",\r\n    \"uri\": \"http://www.google.com\",\r\n    \"author\": \"test\",\r\n    \"points\": 1,\r\n    \"comments\": 1,\r\n    \"rank\": 1\r\n  }\r\n]";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task Test_GetTopPostsIds_ReturnsIds()
        {
            http.Setup(m => m.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult("[1,2,3]"))
                .Verifiable();

            var result = await scraper.GetTopPostsIdsAsync();

            Assert.AreEqual(new[] { 1, 2, 3 }, result);

            http.VerifyAll();
        }

        [Test]
        public void Test_GetTopPostsIds_ThrowsExceptionIfUnableToParse()
        {
            http.Setup(m => m.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult("gibberish"))
                .Verifiable();

            Assert.ThrowsAsync<JsonReaderException>(async () => await scraper.GetTopPostsIdsAsync());

            http.VerifyAll();
        }

        [Test]
        public void Test_GetTopPostsIds_ThrowsExceptionIfRequestFails()
        {
            http.Setup(m => m.GetAsync(It.IsAny<string>()))
                .Throws(new HttpRequestException())
                .Verifiable();

            Assert.ThrowsAsync<HttpRequestException>(async () => await scraper.GetTopPostsIdsAsync());

            http.VerifyAll();
        }

        [Test]
        public async Task Test_GetPostById_ReturnsPost()
        {
            HackerNewsPostResponse post = new HackerNewsPostResponse
            {
                by = "test",
                title = "test",
                url = "http://www.google.com",
                score = 1,
                descendants = 1,
            };
            var response = JsonConvert.SerializeObject(post);

            http.Setup(m => m.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(response))
                .Verifiable();

            var result = await scraper.GetPostById(5);

            Assert.AreEqual(post.by, result.by);
            Assert.AreEqual(post.title, result.title);
            Assert.AreEqual(post.url, result.url);
            Assert.AreEqual(post.score, result.score);
            Assert.AreEqual(post.descendants, result.descendants);

            http.VerifyAll();
        }

        [Test]
        public void Test_GetPostById_ThrowsExceptionIfUnableToParse()
        {
            http.Setup(m => m.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult("gibberish"))
                .Verifiable();

            Assert.ThrowsAsync<JsonReaderException>(async () => await scraper.GetPostById(5));

            http.VerifyAll();
        }

        [Test]
        public void Test_GetPostById_ThrowsExceptionIfRequestFails()
        {
            http.Setup(m => m.GetAsync(It.IsAny<string>()))
                .Throws(new HttpRequestException())
                .Verifiable();

            Assert.ThrowsAsync<HttpRequestException>(async () => await scraper.GetPostById(5));

            http.VerifyAll();
        }

        [Test]
        public void Test_HackerNewsPost_IsValid()
        {
            var post = new HackerNewsPostResponse
            {
                by = "test",
                title = "test",
                url = "http://www.google.com",
                score = 1,
                descendants = 1,
            };

            Assert.IsFalse(scraper.PostIsInvalid(post));
        }

        [Test]
        public void Test_HackerNewsPost_IsNotValid()
        {
            var post1 = new HackerNewsPostResponse
            {
                by = "test",
                title = "test",
                url = "http://www.google.com",
                score = -1,
                descendants = 1,
            };
            var post2 = new HackerNewsPostResponse
            {
                by = "test",
                title = "test",
                url = "http://www.google.com",
                score = 1,
                descendants = -1,
            };
            var post3 = new HackerNewsPostResponse
            {
                by = "test",
                title = "test",
                url = "not a uri",
                score = 1,
                descendants = 1,
            };
            var post4 = new HackerNewsPostResponse
            {
                by = "test",
                title = null,
                url = "http://www.google.com",
                score = -1,
                descendants = 1,
            };
            var post5 = new HackerNewsPostResponse
            {
                by = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                title = "test",
                url = "http://www.google.com",
                score = -1,
                descendants = 1,
            };

            Assert.IsTrue(scraper.PostIsInvalid(post1));
            Assert.IsTrue(scraper.PostIsInvalid(post2));
            Assert.IsTrue(scraper.PostIsInvalid(post3));
            Assert.IsTrue(scraper.PostIsInvalid(post4));
            Assert.IsTrue(scraper.PostIsInvalid(post5));
        }
    }
}