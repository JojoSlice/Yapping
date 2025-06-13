namespace miniReddit.APIManager
{
    public class ChatManager
    {
        private readonly HttpClient _httpClient;
        //private readonly string url ="https://localhost:7188/api/chat/";
        private readonly string url ="https://yappingapi-c6fkeubydcaycdgn.northeurope-01.azurewebsites.net/api/chat/";

        public ChatManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Models.GroupChat>?> GetUserChatsAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"{url}?userId={userId}");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<Models.GroupChat>>();

            return null;
        }

        public async Task<List<Models.ChatMessage>?> GetChatMessagesAsync(string chatId)
        {
            var response = await _httpClient.GetAsync($"{url}messages?chatId={chatId}");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<Models.ChatMessage>>();

            return null;
        }
        public async Task<bool> CreateGroupChatAsync(Models.GroupChat groupChat)
        {
            var response = await _httpClient.PostAsJsonAsync($"{url}group", groupChat);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddUserToChatAsync(string chatId, string userId)
        {
            var response = await _httpClient.PutAsync($"{url}{chatId}/add/{userId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveUserFromChatAsync(string chatId, string userId)
        {
            var response = await _httpClient.PutAsync($"{url}{chatId}/remove/{userId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateChatMessageAsync(Models.ChatMessage message)
        {
            var response = await _httpClient.PostAsJsonAsync($"{url}message", message);
            return response.IsSuccessStatusCode;
        }


    }
}
