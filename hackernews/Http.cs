using System.Net.Http;
using System.Threading.Tasks;

namespace hackernews
{
    public interface IHttp
    {
        Task<string> GetAsync(string url);
    }

    public class Http : IHttp
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> GetAsync(string url)
        {
            string result = string.Empty;

            using (var response = await client.GetAsync(url))
            {
                result = await response.Content.ReadAsStringAsync();
            }

            return result;
        }
    }
}
