using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace miniReddit.APIManager
{
    public class MessageManager
    {
        private readonly HttpClient _httpClient;
        private readonly string url ="https://localhost:7188/api/message/";

        public MessageManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Models.Message>> GetMessages(string id)
        {
            var response = await _httpClient.GetAsync(url + "?id=" + id);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            if (result != null)
            {
                try
                {
                    var messages = JsonSerializer.Deserialize<List<Models.Message>>(result);
                    return messages ?? throw new Exception("Could not deserialize messages");
                }
                catch(Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                    return new();
                }
            }
            else
            {
                Console.WriteLine("No messages found");
                return new List<Models.Message>();
            }
        }

        public async Task SendMessage(Models.Message message)
        {
            Console.WriteLine("Send message: " + message.Text);
            try
            {
                var json = JsonSerializer.Serialize(message);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        public async Task MarkAsRead(string id)
        {
            try
            {
                var response = await _httpClient.PutAsync(url + "?id=" + id, null);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        public async Task<bool> HasUnread(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync(url + "unread?id=" + id);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();

                var isRead = JsonSerializer.Deserialize<Unread>(result);
                if (isRead != null)
                    if (isRead.HasUnread)
                        return true;
                    else
                    {
                        return false;
                    }
                else
                    throw new Exception("Could not dezerialize");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
            }
            return false;
        }

    }
        public class Unread
        {
            public bool HasUnread { get; set; }
        }
}
