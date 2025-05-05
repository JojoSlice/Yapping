using System.Text.Json;
using System.Text;

namespace miniReddit.APIManager
{
    public class UserManager
    {
        private readonly HttpClient _httpClient;
        private readonly Services.AuthenticationService _authentication;
        private readonly string url ="https://localhost:7188/api/user/";
        public UserManager(HttpClient httpClient, Services.AuthenticationService authentication)
        {
            _httpClient = httpClient;
            _authentication = authentication;
        }

        public async Task<bool> LogIn(string username, string password)
        {
            try
            {
                var request = new
                {
                    Username = username,
                    Password = password
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string endpoint = $"{url}Login";
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


        public async Task<string> GetUserIdAsync(string username)
        {
            var response = await _httpClient.GetAsync(url + "GetUserId?username=" + username);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<string>() ?? throw new Exception("User not found");
        }

        public async Task<Models.User> GetLoggedInUserAsync()
        {
            string userID = _authentication.GetLoggedInUserId();
            Console.WriteLine($"{userID} user id?");
            var response = await _httpClient.GetAsync(url + "GetUserFromId?userid=" + userID);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Models.User>() ?? throw new Exception("User not found");
        }
    }
}
