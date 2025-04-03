using System.Net.Http.Headers;
using System.Text;
using Company.Mody.PL.Helper.Bitly;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Company.Mody.PL.Helper.Bitly
{
    public class BitlyService(IOptions<BitlySettings> _options) : IBitlyService
    {
        public async Task<string> ShortenUrl(string longUrl)
        {
            string bitlyAccessToken = _options.Value.AccessToken;
            string apiUrl = "https://api-ssl.bitly.com/v4/shorten";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bitlyAccessToken);

                var content = new StringContent(JsonConvert.SerializeObject(new { long_url = longUrl }), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                return result?.link;
            }
        }
    }
}
