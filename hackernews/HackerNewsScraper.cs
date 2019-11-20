using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;

namespace hackernews
{
    public interface IHackerNewsScraper
    {
        Task<string> GetPostsJson(int n);
    }

    public class HackerNewsScraper : IHackerNewsScraper
    {
        private IHttp _http;

        public HackerNewsScraper(IHttp http)
        {
            _http = http;
        }

        public async Task<string> GetPostsJson(int n)
        {
            var ids = await GetPostIDs(n);
            var posts = new List<Post>();

            if (ids == null)
                return "oops";

            foreach (var id in ids)
            {
                var post = await GetPostById(id);

                //if (PostIsInvalid(post))
                //    continue;

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

        private bool PostIsInvalid(HackerNewsPostResponse post)
        {
            if (string.IsNullOrEmpty(post.title) || post.title.Length > 256 ||
                string.IsNullOrEmpty(post.by) || post.by.Length > 256 ||
                !Uri.TryCreate(post.url, UriKind.Absolute, out _) ||
                post.score < 0 || post.descendants < 0)
                return true;

            return false;
        }

        private async Task<int[]> GetPostIDs(int n)
        {
            var result = await _http.GetAsync("https://hacker-news.firebaseio.com/v0/topstories.json");

            var idArray = JArray.Parse(result);

            var ids = idArray.ToObject<int[]>();

            return ids;
        }

        private async Task<HackerNewsPostResponse> GetPostById(int id)
        {
            var result = await _http.GetAsync($"https://hacker-news.firebaseio.com/v0/item/{id}.json?print=pretty");

            var post = JsonConvert.DeserializeObject<HackerNewsPostResponse>(result);

            return post;
        }
            }
}
