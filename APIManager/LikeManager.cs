using System.Text.Json;

namespace miniReddit.APIManager
{
    public class LikeManager
    {
        private readonly HttpClient _httpClient;
        //private readonly string url ="https://localhost:7188/api/likes/";
        private readonly string url ="https://yappingapi-c6fkeubydcaycdgn.northeurope-01.azurewebsites.net/api/likes/";

        public LikeManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Like(string objid, string userid)
        {
            var response = await _httpClient.PostAsync(url + objid + "/" + userid, null);
        }

        public async Task<List<Models.Likes>> GetLikes(string objid)
        {
            try
            {

                var response = await _httpClient.GetAsync(url + "?objid=" + objid);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();

                if (result != null)
                {
                    var likes = JsonSerializer.Deserialize<List<Models.Likes>>(result);
                    return likes ?? [];
                }
                else throw new Exception("Could not find likes");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Models.Likes>();
            }
        }
    }
}
