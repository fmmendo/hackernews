using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace hackernews
{
    public interface IHttp
    {
        Task<string> GetAsync(string url);
    }

    public class Http : IHttp
    {
        public async Task<string> GetAsync(string url)
        {
            HttpClient client = new HttpClient();

            string result = string.Empty;

            using (var response = await client.GetAsync(url))
            {
                result = await response.Content.ReadAsStringAsync();
            }

            return result;
        }
    }
}
