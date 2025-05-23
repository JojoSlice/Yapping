using System.Text.Json;
using System.Text.Json.Serialization;

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
                try
                {
                    var category = JsonSerializer.Deserialize<Models.Category>(result, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return category ?? throw new Exception("Could not get category");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new Models.Category();
                }
            }
            Console.WriteLine("No category found");
            return new Models.Category();
        }

        public async Task<Models.Category> CategoryCheck(string name)
        {
            var response = await _httpClient.GetAsync($"{url}check?name={name}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            if (result != null)
            {
                var cat = JsonSerializer.Deserialize<Models.Category>(result);
                if (cat != null) return cat;  
            }
            return new Models.Category();
        }

        public async Task<List<Models.Category>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync(url + "allcategories");
            response.EnsureSuccessStatusCode();
        
            var result = await response.Content.ReadAsStringAsync();

            if (result != null)
            {
                var cat = JsonSerializer.Deserialize<List<Models.Category>>(result);
                if (cat != null) return cat;  
            }
            return new List<Models.Category>();

        }
    }
}
