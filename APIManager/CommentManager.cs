using System.Text;
using System.Text.Json;

namespace miniReddit.APIManager
{
    public class CommentManager
    {
        private readonly HttpClient _httpClient;
        //private readonly string url ="https://localhost:7188/api/comments/";
        private readonly string url ="https://yappingapi-c6fkeubydcaycdgn.northeurope-01.azurewebsites.net/api/comments/";

        public CommentManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Models.Comment> GetComment(string id)
        {
            Console.WriteLine("getComment");
            var response = await _httpClient.GetAsync(url + "comment?id=" + id);
            response.EnsureSuccessStatusCode();

            try
            {
                var result = await response.Content.ReadAsStringAsync();
                if(result != null)
                {
                    var comment = JsonSerializer.Deserialize<Models.Comment>(result);
                    return comment ?? new();
                }
                throw new Exception("Json problems");
            }
            catch(Exception ex) { Console.WriteLine(ex.Message); }
            return new();
        }
        
        public async Task<List<Models.Comment>> GetPostComments(string postid)
        {
            Console.WriteLine("GetPostComments--------------");

            var response = await _httpClient.GetAsync(url + "postcomments?postid=" + postid);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            if(result != null)
            {
                try
                {
                    var comments = JsonSerializer.Deserialize<List<Models.Comment>>(result, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    Console.WriteLine(comments);
                    return comments ?? throw new Exception("Could not get comments");
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return new List<Models.Comment>();
                }
            }
            Console.WriteLine("No comments found");
            return new List<Models.Comment>();
        }

        public async Task<bool> NewComment(Models.Comment comment)
        {
            Console.WriteLine($"New comment {comment.Id}");
            if (comment == null) return false;

            try
            {
                var json = JsonSerializer.Serialize(comment);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string endpoint = $"{url}new";
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
