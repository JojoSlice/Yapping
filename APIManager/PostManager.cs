using System.Text.Json;

namespace miniReddit.APIManager
{
    public class PostManager
    {
        private readonly HttpClient _httpClient;
        private readonly string url ="https://localhost:7188/api/posts/";

        public PostManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Models.Post>> GetLatestPosts(DateTime lastPost)
        {
            var response = await _httpClient.GetAsync(url + "latest?lastCreatedAt=" + lastPost);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            if (result != null)
            {
                var posts = JsonSerializer.Deserialize<List<Models.Post>>(result);
                if (posts != null)
                    return posts;
            }
            return new List<Models.Post>();
        }
    }
}
