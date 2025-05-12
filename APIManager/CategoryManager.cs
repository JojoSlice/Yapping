using System.Text.Json;

namespace miniReddit.APIManager
{
    public class CategoryManager
    {
        private readonly HttpClient _httpClient;
        private readonly string url ="https://localhost:7188/api/categories/";

        public CategoryManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Models.Category> GetCategory(string id)
        {
            var response = await _httpClient.GetAsync(url + "get?id=" + id);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            if (result != null)
            {
                var category = JsonSerializer.Deserialize<Models.Category>(result);
                if (category != null)
                    return category;
            }
            return new Models.Category();
        }
    }
}
