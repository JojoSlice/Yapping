using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace miniReddit.APIManager
{
    public class PostManager
    {
        private readonly HttpClient _httpClient;
        private readonly string url ="https://localhost:7188/api/posts/";
        //private readonly string url ="https://yappingapi-c6fkeubydcaycdgn.northeurope-01.azurewebsites.net/api/message/";

        public PostManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Models.Post>> GetLatestPosts(DateTime lastPost)
        {
            var response = await _httpClient.GetAsync(url + "latest?lastCreatedAt=" + Uri.EscapeDataString(lastPost.ToString("o")));
            if (!response.IsSuccessStatusCode)
                return new();

            var result = await response.Content.ReadAsStringAsync();

            if (result != null)
            {
                try
                {
                    var posts = JsonSerializer.Deserialize<List<Models.Post>>(result, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return posts ?? new List<Models.Post>();
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Deserialization error: {ex.Message}");
                    return new List<Models.Post>();
                }
            }
            return new List<Models.Post>();
        }

        public async Task<List<Models.Post>> GetPosts()
        {
            var response = await _httpClient.GetAsync(url + "getposts");
            if (!response.IsSuccessStatusCode)
                return new();

            var result = await response.Content.ReadAsStringAsync();

            if (result != null)
            {
                try
                {
                    var posts = JsonSerializer.Deserialize<List<Models.Post>>(result, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return posts ?? new List<Models.Post>();
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Deserialization error: {ex.Message}");
                    return new List<Models.Post>();
                }
            }
            return new List<Models.Post>();
        }

        public async Task LikePost(string postid)
        {
            try
            {
                var response = await _httpClient.PostAsync(url + postid + "like", null);
                response.EnsureSuccessStatusCode();
            }
            catch (JsonException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<Models.Post> GetPost(string postid)
        {
            Console.WriteLine("GetPost körs nu!!!!!!!!!!!!!!!!");
            try
            {
                var response = await _httpClient.GetAsync(url + "getpost?postid=" + postid);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();

                if (result != null)
                {
                    var post = JsonSerializer.Deserialize<Models.Post>(result);
                    Console.WriteLine(post.Id);
                    if (post != null) return post;
                }
                throw new Exception("No post found");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
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
