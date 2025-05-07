using System.Text.Json;
using System.Text;
using System.Web.Mvc;

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

        public async Task<bool> ChangeUserImg(string id, string path)
        {
            Console.WriteLine("Changie");
            try
            {
                var response = await _httpClient.GetAsync(url + "changePic?id=" + id + "&path=" + path);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
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
            try
            {
                var response = await _httpClient.GetAsync(url + "GetUserId?username=" + username);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();

                return result ?? throw new Exception("User not found");
            }
            catch(Exception ex) 
            {
                return ex.ToString();
            }
        }

        public async Task<Models.User> GetLoggedInUserAsync()
        {
            var userID = _authentication.GetLoggedInUserId();
            var response = await _httpClient.GetAsync(url + "GetUserFromId?userid=" + userID);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(result))
            {
                var user = JsonSerializer.Deserialize<Models.User>(result);
                Console.WriteLine(user + " : " + user.Username + " userDeserialized------------------------------------");
                if (user != null)
                {
                    Console.WriteLine(user.ProfileImg);
                    return user;
                }
                throw new Exception("User not found");
            }
            Console.WriteLine("helvete");
            return new Models.User();
        }
    }
}
