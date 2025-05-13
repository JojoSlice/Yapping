using Microsoft.AspNetCore.Mvc;
using System.Text;
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

        public async Task<bool> CreatePost(Models.Post post)
        {
            try
            {
                var json = JsonSerializer.Serialize(post);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string endpoint = $"{url}post";
                var response = await _httpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
