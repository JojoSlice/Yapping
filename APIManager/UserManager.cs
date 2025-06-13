using System.Text.Json;
using System.Text;

namespace miniReddit.APIManager
{
    public class UserManager
    {
        private readonly HttpClient _httpClient;
        private readonly Services.AuthenticationService _authentication;
        
        private readonly string url ="https://localhost:7188/api/user/";
        //private readonly string url ="https://yappingapi-c6fkeubydcaycdgn.northeurope-01.azurewebsites.net/api/user/";
        public UserManager(HttpClient httpClient, Services.AuthenticationService authentication)
        {
            _httpClient = httpClient;
            _authentication = authentication;
        }

        public async Task<bool> CheckUsername(string username)
        {
            try
            {
                var response = await _httpClient.GetAsync(url + "usernameTaken?username=" + username);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool> CheckEmail(string email)
        {
            try
            {
                var response = await _httpClient.GetAsync(url + "emailTaken?username=" + email);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool> ChangeUserImg(string id, string path)
        {
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

        public async Task<bool> Register(Models.User user)
        {
            try
            {
                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string enpoint = $"{url}Register";
                var response = await _httpClient.PostAsync(enpoint, content);
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

        public async Task<Models.UserInfo> GetUserinfo(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync(url + "Userinfo?id=" + id);
                response.EnsureSuccessStatusCode();

                var resultString = await response.Content.ReadAsStringAsync();
                var userInfo = System.Text.Json.JsonSerializer.Deserialize<Models.UserInfo>(resultString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return userInfo ?? throw new Exception("Deserialization failed");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return new();
            }
        }

        public async Task<List<Models.UserInfo>> GetAllUserinfo()
        {
            try
            {
                var response = await _httpClient.GetAsync(url + "AllUserinfo");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                var userInfos = JsonSerializer.Deserialize<List<Models.UserInfo>>(result);
                
                return userInfos ?? throw new Exception("Deserialization failed");
            }
            catch (Exception ex) { Console.WriteLine("Error" + ex.Message); return new(); }
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
                if (user != null)
                {
                    return user;
                }
                throw new Exception("User not found");
            }
            return new Models.User();
        }

        public async Task<Models.User> GetUserById(string id)
        {
            var response = await _httpClient.GetAsync(url + "GetUserFromId?userid=" + id);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(result))
            {
                var user = JsonSerializer.Deserialize<Models.User>(result);
                if (user != null)
                {
                    return user;
                }
                throw new Exception("User not found");
            }
            return new Models.User();

        }
    }
}
