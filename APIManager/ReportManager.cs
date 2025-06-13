using System.Text;
using System.Text.Json;

namespace miniReddit.APIManager
{
    public class ReportManager
    {
        private readonly HttpClient _httpClient;
        private readonly string url ="https://localhost:7188/api/report/";
        //private readonly string url ="https://yappingapi-c6fkeubydcaycdgn.northeurope-01.azurewebsites.net/api/report/";
        public ReportManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateRaport(Models.Report report)
        {
            try
            {
                var json = JsonSerializer.Serialize(report);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task<List<Models.Report>> GetUnReadReports()
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            if(result != null)
            {
                try
                {
                    var reports = JsonSerializer.Deserialize<List<Models.Report>>(result);
                    return reports ?? new();
                }
                catch(JsonException ex) 
                {
                    Console.WriteLine(ex.Message);
                    return new();
                }
            }
            return new List<Models.Report>();

        }

        public async Task<List<Models.Report>> GetAllReports()
        {
            var response = await _httpClient.GetAsync(url + "all");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            if(result != null)
            {
                try
                {
                    var reports = JsonSerializer.Deserialize<List<Models.Report>>(result);
                    return reports ?? new();
                }
                catch(JsonException ex) 
                {
                    Console.WriteLine(ex.Message);
                    return new();
                }
            }
            return new List<Models.Report>();
        }
    }
}
