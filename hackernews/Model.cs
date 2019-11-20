using System;
using System.Collections.Generic;
using System.Text;

namespace hackernews
{
    public class HackerNewsPostResponse
    {
        public string by { get; set; }
        public int descendants { get; set; }
        public int id { get; set; }
        public List<int> kids { get; set; }
        public int score { get; set; }
        public int time { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string url { get; set; }
    }

    public class Post
    {
        public string title { get; set; }
        public string uri { get; set; }
        public string author { get; set; }
        public int points { get; set; }
        public int comments { get; set; }
        public int rank { get; set; }
    }
}
