using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace hackernews
{
    public interface IHackerNewsScraper
    {
        /// <summary>
        /// Fetches hackernews' top posts and returns a formated JSON string 
        /// containing N posts.
        /// </summary>
        /// <param name="n">Number of posts to return</param>
        /// <returns>JSON string with hacker news posts</returns>
        Task<string> GetPostsJson(int n);
    }

    public class HackerNewsScraper : IHackerNewsScraper
    {
        private IHttp _http;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="http">Injectable HTTP request handler</param>
        public HackerNewsScraper(IHttp http)
        {
            _http = http;
        }

        /// <summary>
        /// Fetches hackernews' top posts and returns a formated JSON string 
        /// containing N posts.
        /// </summary>
        /// <param name="n">Number of posts to return</param>
        /// <returns>JSON string with hacker news posts</returns>
        public async Task<string> GetPostsJson(int n)
        {
            var ids = await GetTopPostsIdsAsync();

            if (ids == null || ids.Count() == 0)
                return "Unable to find any posts.";

            var posts = new List<Post>();

            foreach (var id in ids)
            {
                var post = await GetPostById(id);

                if (PostIsInvalid(post))
                    continue;

                posts.Add(new Post
                {
                    title = post.title,
                    author = post.by,
                    uri = post.url,
                    points = post.score,
                    comments = post.descendants,
                    rank = posts.Count() + 1
                });

                if (posts.Count() == n)
                    break;
            }

            var json = JsonConvert.SerializeObject(posts, Formatting.Indented);

            return json;
        }

        /// <summary>
        /// Checks whether post is invalid.
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        private bool PostIsInvalid(HackerNewsPostResponse post)
        {
            if (string.IsNullOrEmpty(post.title) || post.title.Length > 256 ||
                string.IsNullOrEmpty(post.by) || post.by.Length > 256 ||
                !Uri.TryCreate(post.url, UriKind.Absolute, out _) ||
                post.score < 0 || post.descendants < 0)
                return true;

            return false;
        }

        /// <summary>
        /// Calls the topstories endpoint and returns a parsed array of post ids.
        /// </summary>
        /// <returns></returns>
        private async Task<int[]> GetTopPostsIdsAsync()
        {
            int[] ids;

            try
            {
                var result = await _http.GetAsync("https://hacker-news.firebaseio.com/v0/topstories.json");

                JArray idArray = JArray.Parse(result);
                ids = idArray.ToObject<int[]>();
            }
            catch (Exception)
            {
                return null;
            }

            return ids;
        }

        /// <summary>
        /// Calls the hackernews api and returns the post for the given id.
        /// </summary>
        /// <param name="id">Id of post to fetch</param>
        /// <returns>Deserialized post response</returns>
        private async Task<HackerNewsPostResponse> GetPostById(int id)
        {
            HackerNewsPostResponse post;

            try
            {
                var result = await _http.GetAsync($"https://hacker-news.firebaseio.com/v0/item/{id}.json?print=pretty");

                post = JsonConvert.DeserializeObject<HackerNewsPostResponse>(result);
            }
            catch (Exception)
            {
                return null;
            }

            return post;
        }
    }
}
