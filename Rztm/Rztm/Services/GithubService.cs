using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rztm.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rztm.Services
{
    public class GithubService : IGithubService
    {
        private HttpClient _httpClient;


        public GithubService()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.github.com/")
            };
        }


        public async Task<GithubRelease> GetLatestVersionCodeAsync()
        {
            var response = await _httpClient.GetStringAsync($"repos/Klewerro/RzTM-Mobile/releases/latest");
            var jObject = JObject.Parse(response);
            var result = JsonConvert.DeserializeObject<GithubRelease>(response);

            return result;
        }

    }
}
