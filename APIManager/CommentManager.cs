namespace miniReddit.APIManager
{
    public class CommentManager
    {
        private readonly HttpClient _httpClient;
        private readonly string url ="https://localhost:7188/api/comments/";

        public CommentManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
